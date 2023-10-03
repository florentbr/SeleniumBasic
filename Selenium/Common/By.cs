﻿using Selenium.ComInterfaces;
using Selenium.Internal;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Selenium {

    /// <summary>
    /// Provides a mechanism by which to find elements within a document.
    /// </summary>
    /// <example>
    /// Find an element by id:
    /// <code lang="vbs">	
    /// Set bt = By.Id("id")
    /// driver.FindElement(bt).Click
    /// </code>
    /// Find an element by id or name:
    /// <code lang="vbs">	
    /// Set bt = By.Any(By.Id("name"), By.Id("id"))
    /// driver.FindElement(bt).Click
    /// </code>
    /// </example>
    [ProgId("Selenium.By")]
    [Guid("0277FC34-FD1B-4616-BB19-80B2B91F0D44")]
    [Description("Provides a mechanism by which to find elements within a document.")]
    [ComVisible(true), ClassInterface(ClassInterfaceType.None)]
    public class By : _By {

        private readonly Strategy _strategy;
        private readonly object _value;

        private By(Strategy strategy, string value) {
            _strategy = strategy;
            _value = value;
        }

        private By(params By[] locators) {
            _strategy = Strategy.Any;
            _value = locators;
        }

        /// <summary>
        /// Strategy
        /// </summary>
        public Strategy Strategy {
            get { return _strategy; }
        }

        /// <summary>
        /// Value
        /// </summary>
        public object Value {
            get { return _value; }
        }

        /// <summary>
        /// Gets a string representation of the finder.
        /// </summary>
        /// <returns>The string displaying the finder content.</returns>
        public override string ToString() {
            if (_value == null)
                return string.Empty;

            By[] byany = _value as By[];

            string txt = byany != null ?
                byany.Join(" Or ")
                : string.Format("{0}={1}", _strategy, _value.ToString());

            return txt;
        }

        /// <summary>
        /// Comapre two by objects
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj) {
            var other = obj as By;
            bool equals = other != null
                && _strategy.Equals(other._strategy)
                && _value.Equals(other._value);
            return equals;
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() {
            return _strategy.GetHashCode() ^ _value.GetHashCode();
        }



        internal static string FormatStrategy(Strategy strategy) {
            switch (strategy) {
                case Strategy.Class: return "class name";
                case Strategy.Css: return "css selector";
                case Strategy.Id: return "id";
                case Strategy.Name: return "name";
                case Strategy.LinkText: return "link text";
                case Strategy.PartialLinkText: return "partial link text";
                case Strategy.Tag: return "tag name";
                case Strategy.XPath: return "xpath";
                case Strategy.Any: return "multiple";
            }
            throw new SeleniumException("Unknown strategy: {0}", strategy);
        }

        internal static Strategy ParseStrategy(string strategy) {
            switch (strategy.Remove(' ').ToLower()) {
                case "class": return Strategy.Class;
                case "css": return Strategy.Css;
                case "id": return Strategy.Id;
                case "name": return Strategy.Name;
                case "link": return Strategy.LinkText;
                case "partiallink": return Strategy.PartialLinkText;
                case "tag": return Strategy.Tag;
                case "xpath": return Strategy.XPath;
            }
            throw new SeleniumError("Unknown strategy: {0}", strategy);
        }


        #region Static API

        /// <summary>
        /// Search by id
        /// </summary>
        /// <param name="id">Element id</param>
        /// <returns>By object</returns>
        public static By Id(string id) {
            return new By(Strategy.Id, id);
        }

        /// <summary>
        /// Search by class name
        /// </summary>
        /// <param name="className">Element class name</param>
        /// <returns></returns>
        public static By Class(string className) {
            return new By(Strategy.Class, className);
        }

        /// <summary>
        /// Search by CSS selector
        /// </summary>
        /// <param name="cssSelector">CSS seletor</param>
        /// <returns>By object</returns>
        public static By Css(string cssSelector) {
            return new By(Strategy.Css, cssSelector);
        }

        /// <summary>
        /// Search by link text
        /// </summary>
        /// <param name="linkText">Text of the link</param>
        /// <returns>By object</returns>
        public static By LinkText(string linkText) {
            return new By(Strategy.LinkText, linkText);
        }

        /// <summary>
        /// Search by name attribute
        /// </summary>
        /// <param name="name">Element name</param>
        /// <returns>By object</returns>
        public static By Name(string name) {
            return new By(Strategy.Name, name);
        }

        /// <summary>
        /// Search by partial link text
        /// </summary>
        /// <param name="partialLinkText">Partial link text</param>
        /// <returns>By object</returns>
        public static By PartialLinkText(string partialLinkText) {
            return new By(Strategy.PartialLinkText, partialLinkText);
        }

        /// <summary>
        /// Search by tag name
        /// </summary>
        /// <param name="tagName">Element tag name</param>
        /// <returns>By object</returns>
        public static By Tag(string tagName) {
            return new By(Strategy.Tag, tagName);
        }

        /// <summary>
        /// Search by XPath
        /// </summary>
        /// <param name="xpath">XPath locator</param>
        /// <returns>By object</returns>
        public static By XPath(string xpath) {
            return new By(Strategy.XPath, xpath);
        }

        /// <summary>
        /// Search using multiple mechanisms
        /// </summary>
        /// <param name="by1">Mechanism 1</param>
        /// <param name="by2">Mechanism 2</param>
        /// <param name="by3">Optional - Mechanism 3</param>
        /// <param name="by4">Optional - Mechanism 4</param>
        /// <param name="by5">Optional - Mechanism 5</param>
        /// <param name="by6">Optional - Mechanism 6</param>
        /// <returns>By object</returns>
        public static By Any(By by1, By by2, By by3 = null, By by4 = null, By by5 = null, By by6 = null) {
            return new By(by1, by2, by3, by4, by5, by6);
        }

        #endregion


        #region COM Interface

        /// <summary>
        /// Creates an empty mechanism.
        /// </summary>
        public By()
            : this(Strategy.None, null) { }


        string _By.Strategy {
            get {
                if (_strategy == Strategy.None)
                    return string.Empty;
                return Enum.GetName(typeof(Strategy), _strategy);
            }
        }

        string _By.Value {
            get {
                return (_value ?? string.Empty).ToString();
            }
        }

        /// <summary>
        /// Search by id
        /// </summary>
        /// <param name="id">Element id</param>
        /// <returns>By object</returns>
        By _By.Id(string id) {
            return new By(Strategy.Id, id);
        }

        /// <summary>
        /// Search by class name
        /// </summary>
        /// <param name="className">Element class name</param>
        /// <returns></returns>
        By _By.Class(string className) {
            return new By(Strategy.Class, className);
        }

        /// <summary>
        /// Search by CSS selector
        /// </summary>
        /// <param name="cssSelector">CSS seletor</param>
        /// <returns>By object</returns>
        By _By.Css(string cssSelector) {
            return new By(Strategy.Css, cssSelector);
        }

        /// <summary>
        /// Search by link text
        /// </summary>
        /// <param name="linkText">Text of the link</param>
        /// <returns>By object</returns>
        By _By.LinkText(string linkText) {
            return new By(Strategy.LinkText, linkText);
        }

        /// <summary>
        /// Search by name attribute
        /// </summary>
        /// <param name="name">Element name</param>
        /// <returns>By object</returns>
        By _By.Name(string name) {
            return new By(Strategy.Name, name);
        }

        /// <summary>
        /// Search by partial link text
        /// </summary>
        /// <param name="partialLinkText">Partial link text</param>
        /// <returns>By object</returns>
        By _By.PartialLinkText(string partialLinkText) {
            return new By(Strategy.PartialLinkText, partialLinkText);
        }

        /// <summary>
        /// Search by tag name
        /// </summary>
        /// <param name="tagName">Element tag name</param>
        /// <returns>By object</returns>
        By _By.Tag(string tagName) {
            return new By(Strategy.Tag, tagName);
        }

        /// <summary>
        /// Search by XPath
        /// </summary>
        /// <param name="xpath"><see href="https://www.w3schools.com/xml/xpath_syntax.asp">XPath</see></param>
        /// <returns>By object</returns>
        By _By.XPath(string xpath) {
            return new By(Strategy.XPath, xpath);
        }

        /// <summary>
        /// Search using multiple mechanisms
        /// </summary>
        /// <param name="by1">Mechanism 1</param>
        /// <param name="by2">Mechanism 2</param>
        /// <param name="by3">Optional - Mechanism 3</param>
        /// <param name="by4">Optional - Mechanism 4</param>
        /// <param name="by5">Optional - Mechanism 5</param>
        /// <param name="by6">Optional - Mechanism 6</param>
        /// <returns>By object</returns>
        By _By.Any(By by1, By by2, By by3, By by4, By by5, By by6) {
            return new By(by1, by2, by3, by4, by5, by6);
        }

        #endregion

    }

}
