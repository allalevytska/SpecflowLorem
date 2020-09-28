using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.PageObjects;
using TechTalk.SpecFlow;

namespace SpecflowProject
{
    [Binding]
    [SetUpFixture]
    public class GenerateLoremIpsumSteps
    {
        readonly IWebDriver driver = WebDriverSingleton.getInstance();

        private readonly string url = "https://lipsum.com";
        int all_word_matches = 0;
        int average = 0;

        public void WaitForPageLoadComplete()
        {
            IWait<IWebDriver> wait = new WebDriverWait(driver, TimeSpan.FromMilliseconds(100000000));
            wait.Until(driver1 => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
        }

        [FindsBy(How = How.XPath, Using = "//input[@id='amount']")]
        private readonly IWebElement input_field;

        [FindsBy(How = How.CssSelector, Using = "#lipsum > p")]
        private readonly IWebElement words;

        [FindsBy(How = How.CssSelector, Using = "#lipsum")]
        private readonly IWebElement paragraphs;

        [Given(@"the Lipsum page is open")]
        public void GivenTheLipsumPageIsOpen()
        {
            driver.Manage().Window.Maximize();
            driver.Navigate().GoToUrl(url);
            PageFactory.InitElements(driver, this);
        }

        [When(@"I click on (.*)")]
        public void WhenIClickOn(string type)
        {
            driver.FindElement(By.Id(type)).Click();
        }

        [When(@"enter the (.*)")]
        public void WhenEnterThe(string amount)
        {
            input_field.Clear();
            input_field.SendKeys(amount);
        }

        [When(@"press ""(.*)""")]
        public void WhenPress(string button)
        {
            driver.FindElement(By.CssSelector($"input[value='{button}']")).Click();

        }

        [When(@"I change language to Українська")]
        public void WhenIChangeLanguageToУкраїнська()
        {
            driver.FindElement(By.CssSelector("#Languages > a.uk")).Click();
        }

        [When(@"click ""(.*)"" (.*) times")]
        public void WhenClickTimes(string button, int times)
        {
            string ukr_page = driver.Url;
            WaitForPageLoadComplete();
            string text = "";
            for (int i = 0; i < times; i++)
            {
                driver.FindElement(By.CssSelector($"input[value='{button}']")).Click();
                IList<IWebElement> Paragraphs = driver.FindElements(By.XPath("//div[@id='lipsum']//p"));
                foreach (var paragraph in Paragraphs)
                {
                    all_word_matches += Regex.Matches(paragraph.Text.ToLower(), "lorem").Count;
                }
                
                driver.Navigate().GoToUrl(ukr_page);

            }
            average = all_word_matches / times;
        }

        [Then(@"the result contains (.*) (.*)")]
        public void ThenTheResultContains(int amount, string type)
        {
            WaitForPageLoadComplete();

            if (type == "words" && amount == 0)
            {
                var length = AmountOfWords(words);
                Assert.AreEqual(5, length, $"Expected amout should be 5, but was {length}");
            }
            else if (type == "words" && amount > 0)
            {
                var length = AmountOfWords(words);
                Assert.AreEqual(amount, length, $"Expected amout should be {amount}, but was {length}");
            }
            else
            {   
                var length = paragraphs.FindElements(By.CssSelector("#lipsum > p")).Count;
                Assert.AreEqual(amount, length, $"Expected amout should be {amount}, but was {length}");
            }
        }

        [Then(@"the avarage number of paragraphs containing the word “lorem” is more than (.*)")]
        public void AverageNumberOf(int expected_average)
        {
            Assert.IsTrue(average >= expected_average, $"Expected {expected_average} to be greater then {average}, word matches:  {all_word_matches}");
        }

        [OneTimeTearDown]
        public void CloseChromeDriver()
        {
            driver.Close();
        }

        private  int AmountOfWords(IWebElement element)
        {
            return element.Text.Trim(new char[] { ',', '.' }).Split(' ').Length;
        }
    }
}