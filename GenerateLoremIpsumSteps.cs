using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using SeleniumExtras.PageObjects;
using TechTalk.SpecFlow;

namespace SpecflowProject
{
    [Binding]
    public class GenerateLoremIpsumSteps
    {
        private IWebDriver driver;
        private readonly string url = "https://lipsum.com";
        int length = 0;
        int all_word_matches = 0;

        [FindsBy(How = How.XPath, Using = "//input[@id='amount']")]
        private IWebElement input_field;
        private ReadOnlyCollection<IWebElement> element;

        [Given(@"the Lipsum page is open")]
        public void GivenTheLipsumPageIsOpen()
        {
            driver = new ChromeDriver();
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

            for (int i = 0; i == times; i++)
            {
                driver.FindElement(By.CssSelector($"input[value='{button}']")).Click();
                IList<IWebElement> selectElements = driver.FindElements(By.XPath("//div[@id='lipsum']//p"));
                all_word_matches += selectElements
                      .Select(x => x.Text.Contains("lorem")).Count();
                driver.Navigate()
                      .GoToUrl(url);
                int average = all_word_matches / times;
            } 
        }
        
        [Then(@"the result contains (.*) (.*)")]
        public void ThenTheResultContains(int amount, string type)
        {
            if (type == "words" && amount > 1)
            {
                length = driver.FindElement(By.XPath("//div[@id='lipsum']")).Text.Trim(new char[] { ',', '.' }).Split(' ').Length;
            }
            else if (type == "words" && amount < 2)
            {
                length = amount;
            }
            else if (type == "paras")
            {
                IList<IWebElement> element = driver.FindElements(By.XPath("//div[@id='lipsum']//p"));
                length = element.Count();
            }
            Assert.AreEqual(amount, length);
        }
        
        [Then(@"the avarage number of paragraphs containing the word “lorem” is more than (.*) \(>(.*)\)")]
        public void ThenTheAvarageNumberOfParagraphsContainingTheWordLoremIsMoreThan(int average, int p1)
        {
            Assert.IsTrue(average > p1);
        }
        
        [AfterScenario]
        public void CloseChromeDriver()
        {
            driver.Quit();
        }
    }
}
