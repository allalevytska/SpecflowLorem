using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace SpecflowProject
{
    class WebDriverSingleton
    {
        public static IWebDriver driver;

        public static IWebDriver getInstance()
        {
            if (driver == null)
            {
                driver = new ChromeDriver();
            }
            return driver;
        }
    }
}
