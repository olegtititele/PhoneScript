using System;

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

using PostgreSQL;



class Selenium
{
    public static IWebDriver driver;
    static int Main(string[] args)
    {
        var chromeOptions = new ChromeOptions();
        chromeOptions.AddArgument("ignore-certificate-errors");
        chromeOptions.AddArguments("headless");
        chromeOptions.AddArguments("window-size=1800x900");
        driver = new ChromeDriver(chromeOptions);

        while(true)
        {
            enumeration_of_users();
        }
        return 1;
    }

    static void enumeration_of_users()
    {
        foreach(var user_id in PostDB.get_users_id())
        {
            if(PostDB.get_links_length_without_number(user_id) > 0)
            {
                if(PostDB.state(user_id)==10)
                {
                    Console.WriteLine("da");
                    update_numbers(user_id);
                }
            }
            else
            {
                Console.WriteLine("net");
                System.Threading.Thread.Sleep(1000);
                continue;
            }
            
        }
    }

    static void update_numbers(string user_id)
    {
        foreach(var adv_link in PostDB.get_links(user_id))
        {
            driver.Navigate().GoToUrl(adv_link);
            try
            {
                driver.FindElement(By.XPath("//*[@id=\"reply-form\"]/div/div[2]/div[1]/div/span[3]")).Click();
                string phone_number = driver.FindElement(By.XPath("//*[@id=\"reply-form\"]/div/div[2]/div[1]/div")).GetAttribute("innerText");
                Console.WriteLine(phone_number);
                if(!phone_number.Contains("+27"))
                {
                    phone_number = "+27" + driver.FindElement(By.XPath("//*[@id=\"reply-form\"]/div/div[2]/div[1]/div")).GetAttribute("innerText");
                }
                    
                PostDB.update_phone_number_inhash(user_id, phone_number, adv_link);
                PostDB.update_phone_number_inmain(user_id, phone_number, adv_link);
            }    
            catch (Exception)
            {
                PostDB.delete_broken_advertisement_fromhash(user_id, adv_link);
                PostDB.delete_broken_advertisement_frommain(user_id, adv_link);
            }
        }
    }
}