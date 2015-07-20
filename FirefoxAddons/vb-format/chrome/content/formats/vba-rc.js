/*
 * Formatter for Selenium / Remote Control VBA client.
 */
 
SeleniumIDE.Preferences.setString("enableExperimentalFeatures", "true");
SeleniumIDE.Preferences.setString("disableFormatChangeMsg", "true");

if(typeof(SeleniumIDE.Loader)!='undefined'){
	if(SeleniumIDE.Loader.getTopEditor()!=null){
		SeleniumIDE.Loader.getTopEditor().app.options['enableExperimentalFeatures']='true';
		SeleniumIDE.Loader.getTopEditor().app.options['disableFormatChangeMsg']='true';
	}
}
if(typeof(editor)!='undefined'){
	editor.app.options['enableExperimentalFeatures']='true';
	editor.app.options['disableFormatChangeMsg']='true';
}

this.name = "vba-rc";

function decodeText(text) {
	text = text.replace(/¤/g, '"');
	return text;
}

function convertText(command, converter) {
	var props = ['command', 'target', 'value'];
	for (var i = 0; i < props.length; i++) {
		var prop = props[i];
		command[prop] = converter(command[prop]);
	}
}

/**
 * Parse source and update TestCase. Throw an exception if any error occurs.
 *
 * @param testCase TestCase to update
 * @param source The source to parse
 */
function parse(testCase, source) {
	var cmdStart = options["receiver"] + ".start";
	var cmdStop = options["receiver"] + ".stop";
	var startIndex = source.toLowerCase().indexOf( cmdStart.toLowerCase() );
	var stopIndex = source.toLowerCase().indexOf( cmdStop.toLowerCase() );
	if(startIndex==-1) throw cmdStart + " is missing !";
	if(stopIndex==-1) throw cmdStop + " is missing !";
	
	var doc = source.substr(startIndex + cmdStart.length, stopIndex - startIndex - cmdStart.length)
			.replace(/([ ,])"""/g, '$1"¤') 
			.replace(/""/g, '¤')
			.replace(/" & "/g, '')
			.replace(/(\w+[^"]) & ([^"]\w+)/g, '"${$1}${$2}"')
			.replace(/(\w+[^"]) & \"([^"]*)\"/g, '"${$1}$2"')
			.replace(/\"([^"]*)\" & ([^"]\w+)/g, '"$1${$2}"')
			.replace(/(\w+[^"]) & ([^"]\w+)/g, '"$1$2"')
			.replace(/" & "/g, '');
	
	var commandRegexp = new RegExp('((\\w+) *=)? *' + options["receiver"] + '\\.(\\w+)([\\( ](\"[^\"]*\"|[\\w\\.]+)(\\, *(\"[^\"]*\"|[\\w\\.]+))?)?');
	var commentRegexp = new RegExp("\\' *([^\\n]+)");
	var storevalueRegexp = new RegExp('(\\w+) *= *(\"[^\"]*\")');
	var commandOrCommentOrStoreRegexp = new RegExp("(" + storevalueRegexp.source + ")|(" + commandRegexp.source + ")|(" + commentRegexp.source + ")", 'g');

	var commands = [];
	var commandFound = false;
	var lastIndex;
	while (true) {
		lastIndex = commandOrCommentOrStoreRegexp.lastIndex;
		var docResult = commandOrCommentOrStoreRegexp.exec(doc);
		if (docResult) {
			if (docResult[1]) { // storevalue
				var command = new Command();
				command.skip = docResult.index - lastIndex;
				command.command = 'store';
				command.target = docResult[3].match(/^[\d\.]+$/) ? docResult[3] : docResult[3].replace(/(^[^"]*$)/,'${$1}').replace(/^"([^]*)"$/,'$1' ).replace(/^[\d\.]+$/,'$1' );
				command.value = docResult[2];
				convertText(command, decodeText);					
				commands.push(command);
			}else if (docResult[4]) { // command
				var command = new Command();
				command.skip = docResult.index - lastIndex;
				command.index = docResult.index;				
				command.variable = docResult[6];
				command.command = docResult[7];
				command.target = docResult[9] ? ( docResult[9].match(/^[\d\.]+$/) ? docResult[9] : docResult[9].replace(/(^[^"]*$)/,'${$1}').replace(/^"([^]*)"$/,'$1' ) ) : '';
				command.value = docResult[11] ? ( docResult[11].match(/^[\d\.]+$/) ? docResult[11] : docResult[11].replace(/(^[^"]*$)/,'${$1}').replace(/^"([^]*)"$/,'$1' ) ) : '';
				command.command = command.command.substr(0, 1).toLowerCase() + command.command.substr(1);
				if(command.variable) {
					command.command = command.command.replace(/^get|is/, 'store');
					if(command.target != ''){
						command.value = command.variable;
					}else{
						command.target = command.variable;
					}
				}
				convertText(command, decodeText);					
				commands.push(command);
			} else if(docResult[12]){ // comment
				var comment = new Comment();
				comment.skip = docResult.index - lastIndex;
				comment.index = docResult.index;
				comment.comment = docResult[13];
				commands.push(comment);
			}
		} else {
			break;
		}
	}
	if (commands.length > 0) {
		testCase.commands = commands;
	}else {
		throw "no command found";
	}
}

/**
 * Format an array of commands to the snippet of source.
 * Used to copy the source into the clipboard.
 *
 * @param The array of commands to sort.
 */
function formatCommands(commands) {
	var commandsText = '';
	for (i = 0; i < commands.length; i++) {
		var text = getSourceForCommand(commands[i]);
		commandsText = commandsText + text;
	}
	return commandsText;
}

function encodeText(text) {
    if (text == null) return "";
	text = text.replace(/"/g, '¤');
	return text;
}

function getSourceForCommand(commandObj) {
	var command = null;
	var comment = null;
	var template = '';
	if (commandObj.type == 'command') {
		command = commandObj;
		command = command.createCopy();
		convertText(command, this.encodeText);
		if(command.command == "store"){
			template = options.commandTemplate.replace(/\$\{command\}/g, 
				command.value + " = " + command.target.replace(/(^[^]+)/g, '"$1"')
			);
		}else if(command.command.match(/^store.+/)){
			if(editor.seleniumAPI.Selenium.prototype[ command.command.replace(/^store/, 'is')]){
				command.command = command.command.replace(/^store/, 'is') ;
			}else if(editor.seleniumAPI.Selenium.prototype[ command.command.replace(/^store/, 'get')]){
				command.command = command.command.replace(/^store/, 'get') ;
			}
			template = options.commandTemplate.replace(/\$\{command\}/g,
				(command.value != '' ? command.value : command.target) + " = "
				+ options["receiver"] + "." + command.command + '('
				+ (command.value != '' ? command.target.replace(/(^[^]+)/g, '"$1"') : '' ) + ')'
			);
		}else{
			template = options.commandTemplate.replace(/\$\{command\}/g, 
				options["receiver"] + "." + command.command
				+ ( command.target.match(/^[\d\.]+$/g) ? ' ' + command.target : command.target.replace(/(^[^]+)/g, ' "$1"') )
				+ ( command.value.match(/^[\d\.]+$/g) ? ', ' + command.value : command.value.replace(/(^[^]+)/g, ', "$1"') )
			);
		}
		template = template.replace(/\$\{(\w+)\}/g, '" & $1 & "')
				.replace(/"" & /g, '').replace(/ & ""/g, '')
				.replace(/¤/g, '""');
				
	} else if (commandObj.type == 'comment') {
		template = options.commentTemplate.replace(/\$\{comment\}/g, commandObj.comment );
	}
	return template;
}

/**
 * Returns a string representing the suite for this formatter language.
 *
 * @param testSuite  the suite to format
 * @param filename   the file the formatted suite will be saved as
 */
function formatSuite(testSuite, filename) {
    var suiteClass = /^(\w+)/.exec(filename)[1];
    suiteClass = suiteClass[0].toUpperCase() + suiteClass.substring(1);
    var formattedSuite = options["suiteTemplate"].replace(/\$\{name\}/g, suiteClass);
	var testTemplate = /.*\$\{tests\}.*\n/g.exec(formattedSuite)[0];
	var formatedTests='';
    for (var i = 0; i < testSuite.tests.length; ++i) {
        var testClass = testSuite.tests[i].getTitle();
        formatedTests += testTemplate.replace(/\$\{tests\}/g, testClass );
    }
    return formattedSuite.replace(/.*\$\{tests\}.*\n/g, formatedTests);
}

/**
 * Format TestCase and return the source.
 * The 3rd and 4th parameters are used only in default HTML format.
 *
 * @param testCase TestCase to format
 * @param name The name of the test case, if any. It may be used to embed title into the source.
 * @param saveHeaderAndFooter true if the header and footer should be saved into the TestCase.
 * @param useDefaultHeaderAndFooter Parameter used for only default format.
 */
 
function format(testCase, name, saveHeaderAndFooter, useDefaultHeaderAndFooter) {
    var TcTitle = testCase.getTitle();
	if (TcTitle) {
		TCname=TcTitle;
	}
	
	var text;
	var commandsText = "";
	var testText;
	var i;
	
	for (i = 0; i < testCase.commands.length; i++) {
		var text = getSourceForCommand(testCase.commands[i]);
		commandsText = commandsText + text;
	}
	
	var testText;
	testText = options.testTemplate;
	testText = testText.replace(/\$\{name\}/g, TCname.replace(/\s/, '_')).
		replace(/\$\{baseURL\}/g, testCase.getBaseURL()).
		replace(/\$\{receiver\}/g, options["receiver"]).
		replace(/\$\{browser\}/g, options["browser"]);
		
	var commandsIndex = testText.indexOf("${commands}");
	if (commandsIndex >= 0) {
		var header = testText.substr(0, commandsIndex);
		var footer = testText.substr(commandsIndex + "${commands}".length);
		testText = header + commandsText + footer;
	}
	return testText;
}

/*
 * Optional: The customizable option that can be used in format/parse functions.
 */

this.options = {
	receiver: "selenium",
	browser: "firefox",
	testTemplate:
	'Public Sub ${name}()\n' +
	'  Dim ${receiver} As New SeleniumWrapper.WebDriver\n' +
	'  ${receiver}.start "${browser}", "${baseURL}"\n' +
	'  ${receiver}.setImplicitWait 5000\n\n' +
	'${commands}\n'+
	'  ${receiver}.stop\n' +
	"End Sub",
	suiteTemplate:
	'Public Sub ${name}()\n' +
	'   Call ${tests}\n'+
	"End Sub",
	commandTemplate: "  ${command}\n",
	commentTemplate: "  '${comment}\n"
};

this.configForm = 
	'<description>Variable for Selenium receiver</description>' +
	'<textbox id="options_receiver" />' +
	'<description>Browser</description>' +
	'<textbox id="options_browser" />' +
	'<separator class="groove"/>' +
	'<description>Template for new test</description>' +
	'<textbox id="options_testTemplate" multiline="true" flex="1" rows="8"/>' +
	'<description>Template for new suite</description>' +
	'<textbox id="options_suiteTemplate" multiline="true" flex="1" rows="8"/>';
