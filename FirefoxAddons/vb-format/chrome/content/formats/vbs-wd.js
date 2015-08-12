/*
 * Formatter for Selenium 2 / WebDriver VBS client.
 */

if (!this.formatterType) {  // this.formatterType is defined for the new Formatter system
  // This method (the if block) of loading the formatter type is deprecated.
  // For new formatters, simply specify the type in the addPluginProvidedFormatter() and omit this
  // if block in your formatter.
  var subScriptLoader = Components.classes["@mozilla.org/moz/jssubscript-loader;1"].getService(Components.interfaces.mozIJSSubScriptLoader);
  subScriptLoader.loadSubScript('chrome://selenium-ide/content/formats/webdriver.js', this);
}

this.commandsToSkip = {
    'waitForPageToLoad' : 1,
    'pause': 0
};

this.postFilter = function(originalCommands) {
  var commands = [];
  var rc;
  for (var i = 0; i < originalCommands.length; i++) {
    var c = originalCommands[i];
    if (c.type == 'command') {
      if (this.commandsToSkip[c.command] && this.commandsToSkip[c.command] == 1) {
        //Skip
      } else if (rc = SeleneseMapper.remap(c)) {  //Yes, this IS an assignment Remap
        commands.push.apply(commands, rc);
      } else {
        commands.push(c);
      }
    } else {
      commands.push(c);
    }
  }
  return commands;
};

this.formatFooter = function(testCase) {
  var formatLocal = testCase.formatLocal(this.name);
  formatLocal.footer = options.footer.replace(/\$\{([a-zA-Z0-9_]+)\}/g, function(str, name) {
     return options[name];
  });
  return formatLocal.footer;
}

this.indents = function(num) {
  return '        ';
}

this.testClassName = function (testName) {
  return testName.split(/[^0-9A-Za-z]+/).map(function(x){
    return capitalize(x);
  }).join('');
}

this.testMethodName = function (testName) {
  return testName.replace(/\s/g, '_');
}

this.string = function (value) {
  var str = '', c = 0;
  for (var i = 0, len = value.length; i < len; i++) {
    c = value.charCodeAt(i);
    if (c < 128) {
      str += value[i];
    } else {
      str += '" & CharW(' + c + ') & "';
    }
  }
  str = str.replace(/"/g, '""')
    .replace(/\r\n/g, '" & vbCrLf & "')
    .replace(/\r/g, '" & vbCr & "')
    .replace(/\n/g, '" & vbLf & "')
    .replace(/ & "$/, '')
    .replace(/^" & /, '');
  return '"' + str + '"';
}

this.array = function (value) {
  var values = [];
  for (var i = 0, len = value.length; i < len; i++){
      values.push(string(value[i]));
  }
  return 'Array(' + values.join(', ') + ')';
}

Equals.prototype.toString = function() {
  return this.e1.toString() + ' = ' + this.e2.toString();
};

Equals.prototype.assert = function() {
  return 'Assert.Equals ' + this.e1.toString() + ', ' + this.e2.toString();
};

Equals.prototype.verify = function() {
  return 'WScript.Echo Verify.Equals(' + this.e1.toString() + ', ' + this.e2.toString() + ')';
};

NotEquals.prototype.toString = function() {
  return this.e1.toString() + ' <> ' + this.e2.toString();
};

NotEquals.prototype.assert = function() {
  return 'Assert.NotEquals ' + this.e1.toString() + ', ' + this.e2.toString();
};

NotEquals.prototype.verify = function() {
  return 'WScript.Echo Verify.NotEquals(' + this.e1.toString() + ', ' + this.e2.toString() + ')';
};

this.joinExpression = function(expression) {
  return 'Join(' + expression.toString() + ', ",")';
}

this.statement = function(expression) {
  return expression.toString();
}

this.assignToVariable = function(type, variable, expression) {
  return variable + ' = ' + expression.toString();
}

this.ifCondition = function(expression, callback) {
  return 'If ' + expression.toString() + ' Then ' + callback();
}

this.assertTrue = function(expression) {
  return 'Assert.True ' + expression.toString();
}

this.assertFalse = function(expression) {
  return 'Assert.False ' + expression.toString();
}

this.verifyTrue = function(expression) {
  return 'WScript.Echo Verify.True(' + expression.toString() + ')';
}

this.verifyFalse = function(expression) {
  return 'WScript.Echo Verify.False(' + expression.toString() + ')';
}

RegexpMatch.patternAsRawString = function(pattern) {
  if (pattern != null) {
    var str = pattern.replace(/^\s+/, '')
        .replace(/\s+$/, '')
        .replace(/\\/g, '\\\\')
        .replace(/\"/g, '""');
    return '"' + pattern + '"';
  } else {
    return '""';
  }
};

RegexpMatch.prototype.patternAsRawString = function() {
  return RegexpMatch.patternAsRawString(this.pattern);
};

RegexpMatch.prototype.toString = function() {
  return 'Utils.IsMatch(' + this.expression + ', ' + RegexpMatch.patternToString(this.pattern) + ')';
};

RegexpMatch.prototype.assert = function() {
  return 'Assert.Matches ' + this.expression + ', ' + this.patternAsRawString();
};

RegexpMatch.prototype.verify = function() {
  return 'WScript.Echo Verify.Matches(' + this.expression + ', ' + this.patternAsRawString() + ')';
};

RegexpNotMatch.prototype.patternAsRawString = function() {
  return RegexpMatch.patternAsRawString(this.pattern);
};

RegexpNotMatch.prototype.assert = function() {
  return 'Assert.NotMatches ' + this.expression + ', ' + this.patternAsRawString();
};

RegexpNotMatch.prototype.verify = function() {
  return 'WScript.Echo Verify.NotMatches(' + this.expression + ', ' + this.patternAsRawString() + ')';
};

this.waitFor = function (expression) {
  return 'While Waiter.Not(' + expression.toString() + '): Wend';
}

this.assertOrVerifyFailure = function(line, isAssert) {
  return 'Err.Raise 10000, "erreur"';
}

this.pause = function(milliseconds) {
  return 'driver.Wait ' + parseInt(milliseconds, 10);
}

this.echo = function(message) {
  return 'WScript.Echo ' + xlateArgument(message);
}

this.formatComment = function(comment) {
  return comment.comment.replace(/.+/mg, function(str) {
    return "' " + str;
  });
}

this.keyVariable = function(key) {
  return 'Keys.' + key;
}

this.sendKeysMaping = {
  BKSP: 'Backspace',
  BACKSPACE: 'Backspace',
  TAB: 'Tab',
  ENTER: 'Enter',
  SHIFT: 'Shift',
  CONTROL: 'Control',
  CTRL: 'Control',
  ALT: 'Alt',
  PAUSE: 'Pause',
  ESCAPE: 'Escape',
  ESC: 'Escape',
  SPACE: 'Space',
  PAGE_UP: 'PageUp',
  PGUP: 'PageUp',
  PAGE_DOWN: 'PageDown',
  PGDN: 'PageDown',
  END: 'End',
  HOME: 'Home',
  LEFT: 'Left',
  UP: 'Up',
  RIGHT: 'Right',
  DOWN: 'Down',
  INSERT: 'Insert',
  INS: 'Insert',
  DELETE: 'Delete',
  DEL: 'Delete',
  SEMICOLON: 'Semicolon',
  EQUALS: 'Equal',

  NUMPAD0: 'NumberPad0',
  N0: 'NumberPad0',
  NUMPAD1: 'NumberPad1',
  N1: 'NumberPad1',
  NUMPAD2: 'NumberPad2',
  N2: 'NumberPad2',
  NUMPAD3: 'NumberPad3',
  N3: 'NumberPad3',
  NUMPAD4: 'NumberPad4',
  N4: 'NumberPad4',
  NUMPAD5: 'NumberPad5',
  N5: 'NumberPad5',
  NUMPAD6: 'NumberPad6',
  N6: 'NumberPad6',
  NUMPAD7: 'NumberPad7',
  N7: 'NumberPad7',
  NUMPAD8: 'NumberPad8',
  N8: 'NumberPad8',
  NUMPAD9: 'NumberPad9',
  N9: 'NumberPad9',
  MULTIPLY: 'Multiply',
  MUL: 'Multiply',
  ADD: 'Add',
  PLUS: 'Add',
  SEPARATOR: 'Separator',
  SEP: 'Separator',
  SUBTRACT: 'Subtract',
  MINUS: 'Subtract',
  DECIMAL: 'Decimal',
  PERIOD: 'Decimal',
  DIVIDE: 'Divide',
  DIV: 'Divide',

  F1: 'F1',
  F2: 'F2',
  F3: 'F3',
  F4: 'F4',
  F5: 'F5',
  F6: 'F6',
  F7: 'F7',
  F8: 'F8',
  F9: 'F9',
  F10: 'F10',
  F11: 'F11',
  F12: 'F12',

  META: 'Meta',
  COMMAND: 'Command'
};

/**
 * Returns a string representing the suite for this formatter language.
 *
 * @param testSuite  the suite to format
 * @param filename   the file the formatted suite will be saved as
 */
this.formatSuite = function(testSuite, filename) {
    var suiteClass = /^(\w+)/.exec(filename)[1];
    suiteClass = suiteClass[0].toUpperCase() + suiteClass.substring(1);
    var formattedSuite = options['suiteTemplate'].replace(/\$\{name\}/g, suiteClass);
	var testTemplate = /.*\$\{tests\}.*\n/g.exec(formattedSuite)[0];
	var formatedTests='';
    for (var i = 0; i < testSuite.tests.length; ++i) {
        var testClass = testSuite.tests[i].getTitle();
        formatedTests += testTemplate.replace(/\$\{tests\}/g, testClass );
    }
    return formattedSuite.replace(/.*\$\{tests\}.*\n/g, formatedTests);
}

this.defaultExtension = function() {
  return this.options.defaultExtension;
}

this.options = {
  receiver: 'driver',
  showSelenese: 'false',
  environment: 'firefox',
  header:
    'Class Script\n' +
    '    Dim By, Assert, Verify, Waiter, ${receiver}\n' +
    '    Sub Class_Initialize\n' +
    '        Set By = CreateObject("Selenium.By")\n' +
    '        Set Assert = CreateObject("Selenium.Assert")\n' +
    '        Set Verify = CreateObject("Selenium.Verify")\n' +
    '        Set Waiter = CreateObject("Selenium.Waiter")\n' +
    '        Set ${receiver} = CreateObject("Selenium.WebDriver")\n' +
    '        ${receiver}.Start "${environment}", "${baseURL}"\n\n',
  footer:
    '    End Sub\n' +
    '\n' +
    '    Sub Class_Terminate\n' +
    '        ${receiver}.Quit\n' +
    '    End Sub\n' +
    'End Class\n' +
    'Set s = New Script\n',
  suiteTemplate:
	'Set oShell = CreateObject("WScript.Shell")\n' +
	'oShell.Run "wscript ${tests}", ,true\n',
  indent:  '1',
  initialIndents: '0',
  defaultExtension: 'vbs'
};

this.configForm =
    '<description>Variable for Selenium instance</description>' +
    '<textbox id="options_receiver" />' +
    '<description>Browser</description>' +
	'<textbox id="options_environment" />' +
	'<description>Header</description>' +
	'<textbox id="options_header" multiline="true" flex="1" rows="4"/>' +
	'<description>Footer</description>' +
	'<textbox id="options_footer" multiline="true" flex="1" rows="4"/>' +
	'<checkbox id="options_showSelenese" label="Show Selenese"/>';

this.name = 'vbs-wd';
this.testcaseExtension = '.vbs';
this.suiteExtension = '.vbs';
this.webdriver = true;

WDAPI.Driver = function() {
  this.ref = options.receiver;
};

WDAPI.Driver.searchContext = function(locatorType, locator) {
  var locatorString = '(' + xlateArgument(locator) + ')';
  switch (locatorType) {
    case 'xpath':
      return 'XPath' + locatorString.replace(/""/g, "'");
    case 'class_name':
      return 'Class' + locatorString;
    case 'css':
      return 'Css' + locatorString.replace(/""/g, "'");
    case 'id':
      return 'Id' + locatorString;
    case 'link':
      return 'LinkText' + locatorString;
    case 'name':
      return 'Name' + locatorString;
    case 'tag_name':
      return 'Tag' + locatorString;
  }
  throw 'Error: unknown strategy [' + locatorType + '] for locator [' + locator + ']';
};

WDAPI.Driver.prototype.back = function() {
  return this.ref + '.GoBack';
};

WDAPI.Driver.prototype.setImplicitWait = function(timeout) {
  return this.ref + '.Timeouts.ImplicitWait = ' + timeout;
};

WDAPI.Driver.prototype.close = function() {
  return this.ref + '.Close';
};

WDAPI.Driver.prototype.findElement = function(locatorType, locator) {
  return new WDAPI.Element(this.ref + '.FindElementBy' + WDAPI.Driver.searchContext(locatorType, locator));
};

WDAPI.Driver.prototype.findElements = function(locatorType, locator) {
  return new WDAPI.ElementList(this.ref + '.FindElementsBy' + WDAPI.Driver.searchContext(locatorType, locator));
};

WDAPI.Driver.prototype.getCurrentUrl = function() {
  return this.ref + '.Url';
};

WDAPI.Driver.prototype.get = function(url) {
  return this.ref + '.Get ' + url;
};

WDAPI.Driver.prototype.getTitle = function() {
  return this.ref + '.Title';
};

WDAPI.Driver.prototype.getAlert = function() {
  return this.ref + '.SwitchToAlert().Text'
};

WDAPI.Driver.prototype.chooseOkOnNextConfirmation = function() {
  return this.ref + '.SwitchToAlert().Accept';
};

WDAPI.Driver.prototype.chooseCancelOnNextConfirmation = function() {
  return this.ref + '.SwitchToAlert().Dismiss';
};

WDAPI.Driver.prototype.refresh = function() {
  return this.ref + '.Refresh';
};

WDAPI.Element = function(ref) {
  this.ref = ref;
};

WDAPI.Element.prototype.clear = function() {
  return this.ref + '.Clear';
};

WDAPI.Element.prototype.click = function() {
  return this.ref + '.Click';
};

WDAPI.Element.prototype.getAttribute = function(attributeName) {
  return this.ref + '.Attribute(' + xlateArgument(attributeName) + ')';
};

WDAPI.Element.prototype.getText = function() {
  return this.ref + '.Text';
};

WDAPI.Element.prototype.isDisplayed = function() {
  return this.ref + '.IsDisplayed';
};

WDAPI.Element.prototype.isSelected = function() {
  return this.ref + '.IsSelected';
};

WDAPI.Element.prototype.sendKeys = function(text) {
  return this.ref + '.SendKeys ' + xlateArgument(text);
};

WDAPI.Element.prototype.submit = function() {
  return this.ref + '.Submit';
};

WDAPI.Element.prototype.select = function(locator) {
  switch(locator.type){
    case 'index':
      var index = locator.string;
      return this.ref + '.AsSelect.SelectByIndex ' + index;
    case 'label':
      var text = xlateArgument(locator.string.replace(/^regexp:(\\s)?/, ''));
      return this.ref + '.AsSelect.SelectByText ' + text;
    case 'value':
      var value = xlateArgument(locator.string);
      return this.ref + '.AsSelect.SelectByValue ' + value;
    default:
      throw 'Error: locator not implemented: ' + locator.type + '=' + locator.string;
  }
};

WDAPI.ElementList = function(ref) {
  this.ref = ref;
};

WDAPI.ElementList.prototype.getItem = function(index) {
  return this.ref + '(' + index + ')';
};

WDAPI.ElementList.prototype.getSize = function() {
  return this.ref + '.Count';
};

WDAPI.ElementList.prototype.isEmpty = function() {
  return this.ref + '.Count = 0';
};

WDAPI.Utils = function() {
};

WDAPI.Utils.isElementPresent = function(how, what) {
  return 'driver.IsElementPresent(By.' + WDAPI.Driver.searchContext(how, what) + ')';
};

WDAPI.Utils.isAlertPresent = function() {
  return this.ref + '.SwitchToAlert(0, False) Is Nothing';
};
