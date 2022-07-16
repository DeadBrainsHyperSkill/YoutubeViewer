using PuppeteerSharp;
using PuppeteerExtraSharp;
using PuppeteerExtraSharp.Plugins.AnonymizeUa;
using PuppeteerExtraSharp.Plugins.ExtraStealth;

Console.Write("Stream URL: ");
string streamURL = Console.ReadLine();

foreach (var cookieFile in Directory.GetFiles(Environment.CurrentDirectory + @"\Cookies"))
{
    string[] cookiesRaw = File.ReadAllLines(cookieFile);
    List<CookieParam> cookieParams = new();
    for (int i = 0; i < cookiesRaw.Length; i++)
    {
        string[] cookie = cookiesRaw[i].Split('\t');
        try
        {
            cookieParams.Add(new CookieParam() { Domain = cookie[0], Name = cookie[5].Replace("_", ""), Value = cookie[6] });
        }
        catch (IndexOutOfRangeException)
        {
            cookieParams.Remove(cookieParams[cookieParams.Count - 1]);
            continue;
        }
    }

    var extra = new PuppeteerExtra();
    extra.Use(new AnonymizeUaPlugin()).Use(new StealthPlugin());

    var browser = await extra.LaunchAsync(new LaunchOptions()
    {
        Headless = true,
        ExecutablePath = @"C:\Program Files\Google\Chrome\Application\chrome.exe"
    });

    var page = await browser.NewPageAsync();
    await page.SetCookieAsync(cookieParams.ToArray());
    await page.GoToAsync(streamURL);
    if (page.QuerySelectorAsync("#img") == null)
    {
        await page.CloseAsync();
        await browser.CloseAsync();
        continue;
    }
    await page.WaitForSelectorAsync(".ytp-play-button");
    await page.ClickAsync(".ytp-play-button");
    await page.ScreenshotAsync(Guid.NewGuid().ToString() + ".png");
}