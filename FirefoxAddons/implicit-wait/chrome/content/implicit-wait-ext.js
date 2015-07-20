/*
 * Copyright 2014 Florent Breheret
 * http://code.google.com/p/selenium-implicit-wait/
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

/**
 * Specifies the amount of time it should wait when searching for an element if it is not immediately present.
 * @param {Integer} timeout Timeout in millisecond, set 0 to disable it
 * @exemple
     setImplicitWait | 0
     setImplicitWait | 1000
 */
Selenium.prototype.doSetImplicitWait = function(timeout){   
    this.browserbot.implicitwait.waitElement_timeout = +timeout || 0;
};

/**
 * Specifies the amount of time it should wait for a condition to be true before executing the command.
 * @param {Integer} timeout Timeout in millisecond, set 0 to disable it
 * @param {String} condition_js Javascript logical expression that need to be true to execute each command.
 * @exemple
     setImplicitWaitCondition |  0  |  
     setImplicitWaitCondition |  1000  | !window.Sys || !window.Sys.WebForms.PageRequestManager.getInstance().get_isInAsyncPostBack();
     setImplicitWaitCondition |  1000  | !window.dojo || !window.dojo.io.XMLHTTPTransport.inFlight.length;
     setImplicitWaitCondition |  1000  | !window.Ajax || !window.Ajax.activeRequestCount;
     setImplicitWaitCondition |  1000  | !window.tapestry || !window.tapestry.isServingRequests();
     setImplicitWaitCondition |  1000  | !window.jQuery || !window.jQuery.active;
 */
Selenium.prototype.doSetImplicitWaitCondition = function(timeout, condition_js) {
    var implicitwait = this.browserbot.implicitwait;
    if((implicitwait.preCondition_timeout = +timeout || 0)){
        implicitwait.preCondition = new Function('return ' + condition_js);
        implicitwait.preCondition.__source__ = condition_js;
    }else{
        implicitwait.preCondition = null;
    }
};


if(this.TestLoop){
    
    this.ImplicitWait = function(){
        this.waitElement_timeout = (window.editor && window.editor.implicitwait.timeout) || 0;
        this.preCondition_timeout = 0;
        this.preCondition = null;
    };
    
    /**
     * Overrides BrowserBot.prototype.findElement: function(locator, win) in selenium-browserbot.js line 1524
     */
    this.MozillaBrowserBot.prototype.findElementOrNull = function(locator, win){
        var loc = this.locator_cache || (this.locator_cache = parse_locator(locator));    //cache the parsing result of the locator. Clearing is done by resume.
        if(!win)
            win = this.getCurrentWindow();
        try{
            var element = this.findElementRecursive(loc.type, loc.string, win.document, win);
            if(element)
                return core.firefox.unwrap(element);
        }catch(e){
            if(e.isSeleniumError)
                throw e;
        }
        this.is_element_missing = true; //boolean used by "resume" to identify that the command failed to find an element
        return null;
    };
    
    /**
     * Overrides TestLoop.prototype.resume: function() in selenium-executionloop.js line 71
     */
    this.TestLoop.prototype.resume = function(){
        var selDebugger = this.selDebugger = window.editor.selDebugger,
            runner = selDebugger.runner,
            selenium = this.selenium = runner.selenium,
            browserbot = this.browserbot = selenium.browserbot,
            implicitwait = browserbot.implicitwait || (browserbot.implicitwait = new runner.ImplicitWait()),
            LOG = runner.LOG;
        
        LOG.debug("currentTest.resume() - actually execute modified");
        browserbot.runScheduledPollers();
        
        var command = this.currentCommand;
        LOG.info("Executing: |" + command.command + " | " + command.target + " | " + command.value);

        if((this.commandHandler = this.commandFactory.getCommandHandler(command.command)) === null)
            throw new SeleniumError("Unknown command: '" + command.command + "'");
            
        command.target = selenium.preprocessParameter(command.target);
        command.value = selenium.preprocessParameter(command.value);
        LOG.debug("Command found, going to execute " + command.command);
        
        runner.updateStats(command.command);
        browserbot.locator_cache = null;     //Clears the locator cache set by findElementOrNull
        
        this.waitElement_timeout = implicitwait.waitElement_timeout;
        this.preCondition = implicitwait.preCondition;        //defined by doSetImplicitWaitCondition
        this.preCondition_endtime = implicitwait.preCondition && new Date().getTime() + implicitwait.preCondition_timeout;
        return this.resume_pre_condition();
    };
    
    /**
     * Waits for the condition defined by doSetImplicitWaitCondition to be true or throws an error if the timeout is reached
     */
    this.TestLoop.prototype.resume_pre_condition = function(){
        var failureMessage;
        try{
            if(!this.preCondition || this.preCondition.call(this.selenium)){
                this.waitElement_endtime = this.waitElement_timeout && new Date().getTime() + this.waitElement_timeout;
                return this.resume_command();
            }
            if(new Date().getTime() < this.preCondition_endtime)
                return this.retry(this.resume_pre_condition, 20);
            failureMessage = 'Implicit condition timeout: ' + this.preCondition.__source__;
        }catch(e){
            failureMessage = 'Exception on implicit condition: ' + extractExceptionMessage(e);
        }
        LOG.error(failureMessage);
        if(this.commandError(failureMessage))
            return this.continueTest();
        return this.testComplete();
    };
    
    /**
     * Executes the current command until there is no failing locator or until the time-out is reached
     */
    this.TestLoop.prototype.resume_command = function(){
        this.browserbot.is_element_missing = false;     //boolean set to true by findElementOrNull when a locator is not found
        try{
            this.result = this.commandHandler.execute(this.selenium, this.currentCommand);
            if(this.browserbot.is_element_missing && this.waitElement_endtime && this.result.failed && new Date().getTime() < this.waitElement_endtime)
                return this.retry(this.resume_command, 20);    //Retry for the verify and assert commands with a failing locator
            return this.resume_term_condition();
        }catch(e){
            if(this.browserbot.is_element_missing && this.waitElement_endtime && new Date().getTime() < this.waitElement_endtime)    //Retry for actions with a failing locator
                return this.retry(this.resume_command, 20);
            var failureMessage = e.isSeleniumError ? e.message : extractExceptionMessage(e);
            if (e.isSeleniumError)
                LOG.error(failureMessage);
            else
                LOG.exception(e);
            if(this.commandError(failureMessage))
                return this.continueTest();
            return this.testComplete();
        }
    };
    
    /**
     * Waits for the termination condition to be true or throws an error if the time-out is reached
     */
    this.TestLoop.prototype.resume_term_condition = function(){
        try{
            this.browserbot.runScheduledPollers();
            if(this.result.terminationCondition && !this.result.terminationCondition())
                return this.retry(this.resume_term_condition, 20);
        }catch(e){
            this.result = {failed: true, failureMessage: extractExceptionMessage(e)};
        }
        this.commandComplete(this.result);
        return this.continueTest();
    };
    
    /**
     * Registers a callback method that will be called after the given delay on the current scope if the editor is not paused.
     */
    this.TestLoop.prototype.retry = function(callback, delay){
        var self = this;
        if(this.selDebugger.state !== 2) // if not PAUSE_REQUESTED
            window.setTimeout(function(){callback.call(self);}, delay);
    };

}