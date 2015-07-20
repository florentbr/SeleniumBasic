/*
 * Copyright 2012 Florent Breheret
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

(function(){

var DEFAULT_TIMEOUT = 5000;


/**
 * Specifies the amount of time it should wait when searching for an element if it is not immediately present.
 * @param {Integer} timeout Timeout in millisecond, set 0 to disable it
 * @exemple
     setImplicitWait | 0
     setImplicitWait | 1000
 */
Selenium.prototype.doSetImplicitWait = function(timeout){
    implicitwait.setImplicitWait(timeout);
};


/**
 * Specifies the amount of time it should wait for a condition to be true to continue to the next command.
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
    implicitwait.setImplicitWaitCondition(timeout, condition_js);
}


/** Overrides findElement: function(locator, win) in selenium-browserbot.js line 1524 */
BrowserBot.prototype.findElement
 = MozillaBrowserBot.prototype.findElement
 = KonquerorBrowserBot.prototype.findElement
 = SafariBrowserBot.prototype.findElement
 = OperaBrowserBot.prototype.findElement
 = IEBrowserBot.prototype.findElement
 = function (locator, win){
    var element = base.findElementOrNull(locator, win);
    if(element === null) 
        throw new ElementNotFountError(locator);
    return window.core.firefox.unwrap(element);
};


/**
 * Class: Adds the implicit wait feature.
 * @param {Object} editor
 */
function ImplicitWait(owner){
    this.owner = owner;
}
ImplicitWait.prototype = {
    
    wait_timeout: 0,
    postcondition_timeout: 0,
    postcondition_func: null,
    postcondition_run: null,
    
    /** Call from the setImplicitWait command*/
    setImplicitWait: function(timeout){
        this.wait_timeout = +timeout || 0;
    },
        
    /** Call from the setImplicitWaitCondition command*/
    setImplicitWaitCondition: function(timeout, condition_js){
        if((this.postcondition_timeout = +timeout || 0)){
            this.postcondition_func = new Function('return ' + condition_js);
            this.postcondition_func.__string__ = condition_js;
        }else{
            this.postcondition_func = null;
        }
        this.postcondition_run = null;
    },
    
    /** Overrides TestLoop.prototype.resume: function() in selenium-executionloop.js line 71 */
    wrap_TestLoop_resume: function(base, fn, args/*[]*/){
        var selDebugger = this.owner.selDebugger,
            runner = selDebugger.runner,
            selenium = runner.selenium,
            browserbot = selenium.browserbot,
            LOG = runner.LOG;
        
        LOG.debug("currentTest.resume() - actually execute modified");
        browserbot.runScheduledPollers();
        
        var command = base.currentCommand;
        LOG.info("Executing: |" + command.command + " | " + command.target + " | " + command.value + " |");

        var handler = base.commandFactory.getCommandHandler(command.command);
        if (handler === null)
            throw new SeleniumError("Unknown command: '" + command.command + "'");
            
        command.target = selenium.preprocessParameter(command.target);
        command.value = selenium.preprocessParameter(command.value);
        LOG.debug("Command found, going to execute " + command.command);
        
        var locator_endtime = this.wait_timeout && new Date().getTime() + this.wait_timeout;
        var self = this;
        (function loopFindElement(){
            try{
                base.result = handler.execute(selenium, command);
                base.waitForCondition = base.result.terminationCondition;
                (function loopCommandCondition(){    //handles the andWait condition in replacement of continueTestWhenConditionIsTrue
                    try{
                        browserbot.runScheduledPollers();
                        if(base.waitForCondition && !base.waitForCondition())
                            return selDebugger.state !== 2/*PAUSE_REQUESTED*/ && window.setTimeout(loopCommandCondition, 15);
                        base.waitForCondition = null;
                        var postcondition_endtime = self.postcondition_run && new Date().getTime() + self.postcondition_timeout;
                        self.postcondition_run = self.postcondition_func;
                        (function loopPostCondition(){    //handles the customized postcondition
                            if(postcondition_endtime){
                                try{
                                    if(new Date().getTime() > postcondition_endtime)
                                        base.result = {failed: true, failureMessage: 'Timed out on postcondition ' + self.postcondition_func.__string__};
                                    else if(!self.postcondition_func.call(selenium))
                                        return selDebugger.state !== 2/*PAUSE_REQUESTED*/ && window.setTimeout(loopPostCondition, 15);
                                }catch(e){
                                     base.result = {failed: true, failureMessage: 'Exception on postcondition ' + self.postcondition_func.__string__ + '  Exception:' + extractExceptionMessage(e)};
                                }
                            }
                            base.commandComplete(base.result);
                            base.continueTest();
                        })();
                    }catch(e){
                        base.result = {failed: true, failureMessage: extractExceptionMessage(e)};
                        base.commandComplete(base.result);
                        base.continueTest();
                    }
                })();
            }catch(e){
                if(e.isElementNotFoundError && locator_endtime && new Date().getTime() < locator_endtime)
                    return selDebugger.state !== 2/*PAUSE_REQUESTED*/ && window.setTimeout(loopFindElement, 20);
                if(base._handleCommandError(e))
                    base.continueTest();
                else
                    base.testComplete();
            }
        })();
    }
};


/**
 * Class: Error specific to findElement
 * @param {String} locator
 */
function ElementNotFountError(locator) {
    this.locator = locator;
}
ElementNotFountError.prototype = {
    isSeleniumError: true,
    isElementNotFoundError: true,
    get message(){ return 'Element ' + this.locator + ' not found'; },
    toString: function(){ return this.message; }
};


/**
 * Wraps a method call to a function on a specified context. Skips the wrapping if already done.
 * @param {Object} obj Object containing the function to intercept
 * @param {String} key Name of the function to intercept
 * @param {Object} context Object on which the intercepting function func will be applied
 * @param {Function} func  Function intercepting obj[method] : function(context, function, arguments)
 */
function wrap(obj, key, context, func){
    var fn = obj[key], w;
    if(!(w=fn.__wrap__) || w.src !== fn || w.tgt !== func ){
        (obj[key] = function(){
            return func.call(context, this, fn, arguments);
        }).__wrap__ = {src:fn, tgt:func};
    }
}


//Instantiates the plug-in
var implicitwait = new ImplicitWait(this.htmlTestRunner);
if(window.RemoteRunner)
    window.RemoteRunner.prototype.resume = wrap(window.RemoteRunner.prototype, 'resume', implicitwait, ImplicitWait.prototype.wrap_TestLoop_resume);
if(window.HtmlRunnerTestLoop)
    window.HtmlRunnerTestLoop.prototype.resume =  wrap(window.HtmlRunnerTestLoop.prototype, 'resume', implicitwait, ImplicitWait.prototype.wrap_TestLoop_resume);

})();