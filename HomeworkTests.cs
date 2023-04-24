using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;


namespace DanilovAE_Homework;

public class HomeworkTests
{
    private ChromeDriver driver;
    private WebDriverWait wait;
    private const string staffUrl = "https://staff-testing.testkontur.ru";
    

    [SetUp]
    public void Setup()
    {
        var options = new ChromeOptions();
        options.AddArguments("--no-sandbox", "--start-maximized", "--disable-extensions");
        driver = new ChromeDriver(options);
        wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
        Authorization();
    }


    private void Authorization()
    {
        driver.Navigate().GoToUrl(staffUrl);
        var login = wait.Until(driver=>driver.FindElement(By.Id("Username")));
        
        login.SendKeys("kss6655@mail.ru");
        var password = driver.FindElement((By.Id("Password")));
        password.SendKeys("Zerx14589652.");
        var enter = driver.FindElement(By.Name("button"));
        enter.Click();

        wait.Until(x => x.Url == $"{staffUrl}/news");
    }

    [Test]
    public void AuthorizationTest()
    {
        Assert.AreEqual($"{staffUrl}/news", driver.Url,
            $"После клика авторизации урл страницы не совпадает с {staffUrl}/news");    
    }

    [Test]
    public void NavigationMenu()
    {
        var community = driver.FindElement(By.CssSelector("[data-tid='Community']"));
        community.Click();
        
        Assert.AreEqual($"{staffUrl}/communities",driver.Url,"Переход по меню не работает");
    }

    [Test]
    public void SearchBar()
    {
        var searchBar = driver.FindElement(By.CssSelector("[data-tid='SearchBar']"));
        searchBar.Click();
        
        var input = searchBar.FindElement(By.ClassName("react-ui-1oilwm3"));
        input.SendKeys("Агапова Алиса Алексеевна");
        
        var dropdownList = wait.Until(x=>x.FindElement(By.CssSelector("[data-tid='ScrollContainer__inner']")));

        dropdownList.Click();
       
        Assert.AreEqual($"{staffUrl}/profile/f23f7980-6b93-4959-9fc0-dbc3359c0dbb",
           driver.Url,"Страница не найдена");
    }

    [Test]
    public void SendMessage()
    {
        var message = $"Test message {Guid.NewGuid()}";
        driver.Navigate().GoToUrl($"{staffUrl}/messages/00026074-b53b-4f06-897b-b12158d48014");
        
        var commentInput = wait.Until(x=>x.FindElement(By.CssSelector("[data-tid='CommentInput']")));

        commentInput.Click();
       
        commentInput.SendKeys(message);

        var sendButton = driver.FindElement(By.CssSelector("[data-tid='SendButton']"));
        sendButton.Click();

        var result = wait.Until(x => x.FindElements(By.CssSelector("[data-tid='MessageText']")).Last().Text == message);
        
        Assert.AreEqual(true,result,"Сообщение не отправилось");
    }
    
    [Test]
    public void WriteComment()
    {
        /* Я думаю, что проблемы могут возникнуть потому, что тестовый стенд не откатывается до начального состояния.
         * Могут произойти изменения у поста, под которым пишется коммент.
         * Можно упереться в максимально допустимое количество комментариев под постом. 
         */
            
        var testComment = $"Test comment {Guid.NewGuid()}";
        driver.Navigate().GoToUrl(
            $"{staffUrl}/communities/18c18e9c-59dd-4d51-ad99-96261e81a5d3?tab=discussions&id=8c04521d-cc2e-488f-952d-30cb84f34a73");
        
        var addComment = wait.Until(x => x.FindElement(By.CssSelector("[data-tid='AddComment']")));
        addComment.Click();
      
        var inputComment = wait.Until(x => x.FindElement(By.CssSelector("[data-tid='CommentInput']")));
        inputComment.Click();
        inputComment.SendKeys(testComment);

        var sendComment = driver.FindElement(By.ClassName("react-ui-m0adju"));
        sendComment.Click();

        var result = wait.Until(x => x.FindElements(By.CssSelector("[data-tid='TextComment']")).Last().Text == testComment);
        
        Assert.AreEqual(true,result,"Комментарий не отправился");
    }

    [TearDown]
    public void TearDown()
    {
        driver.Quit();
    }

}