using Kartrider_Parts_Crawler.Models;

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
namespace Kartrider_Parts_Crawler
{
    class Program
    {
        static readonly HashSet<string> _partEngine = File.ReadAllLines(@"partsData\partsEngine.txt").ToHashSet();
        static readonly HashSet<string> _partHandle = File.ReadAllLines(@"partsData\partsHandle.txt").ToHashSet();
        static readonly HashSet<string> _partWheel = File.ReadAllLines(@"partsData\partsWheel.txt").ToHashSet();
        static readonly HashSet<string> _partKit = File.ReadAllLines(@"partsData\partsKit.txt").ToHashSet();
        /// <summary>
        /// 진입점
        /// </summary>
        /// <param name="args">닉네임 리스트</param>
        static void Main(string[] args)
        {
            string[] nicknames = args;
            List<Parts> result = new List<Parts>();
            foreach (string nickname in nicknames)
            {
                List<Parts> parts = GetParts(nickname);
                result.AddRange(parts);
            }
            ToJson(result.Where(p => p.PartsType == PartsType.Engine).Distinct().OrderBy(p => p.Key), "PartsEngine.json");
            ToJson(result.Where(p => p.PartsType == PartsType.Wheel).Distinct().OrderBy(p => p.Key), "PartsWheel.json");
            ToJson(result.Where(p => p.PartsType == PartsType.Kit).Distinct().OrderBy(p => p.Key), "PartsKit.json");
            ToJson(result.Where(p => p.PartsType == PartsType.Handle).Distinct().OrderBy(p => p.Key), "PartsHandle.json");
        }
        static void ToJson(IEnumerable<Parts> parts, string filename)
        {
            JsonSerializerOptions jso = new JsonSerializerOptions
            {
                // 보기 좋게
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
            string json = JsonSerializer.Serialize(parts, jso);
            File.WriteAllText(filename, json);
        }
        static List<Parts> GetParts(string nickname)
        {
            List<Parts> result = new List<Parts>();
            ChromeOptions options = new ChromeOptions();
            // 크롬 숨기기
            options.AddArgument("headless");
            IWebDriver driver = new ChromeDriver(options);
            // 무한 반복, 탈출 조건은 68 라인 참고
            for (int i = 1; ; i++)
            {
                // 차고 아이템 URL
                string mycarsUrl = $"https://kart.nexon.com/Garage/Item?strRiderID={nickname}&page={i}";
                driver.Url = mycarsUrl;
                // 현재 페이지에서 아이템 엘리먼트들 가져오기
                var items = driver.FindElements(By.XPath("//*[@id=\"CntItemSec\"]/ul/li"));
                foreach (var item in items)
                {
                    // 아이템 이름
                    string name = item.FindElement(By.XPath("span[2]")).Text;
                    /*
                     * 한 페이지에 표시되는 아이템은 총 8개로 아이템이 비어 있는 경우 ""
                     * 비어 있다는 건 더이상 아이템이 없다는 것을 의미함
                     * 반복문 탈출
                     */
                    if (name == "")
                    {
                        goto End;
                    }
                    PartsType partsType;
                    if (_partEngine.Contains(name))
                    {
                        partsType = PartsType.Engine;
                    }
                    else if (_partHandle.Contains(name))
                    {
                        partsType = PartsType.Handle;
                    }
                    else if (_partWheel.Contains(name))
                    {
                        partsType = PartsType.Wheel;
                    }
                    else if (_partKit.Contains(name))
                    {
                        partsType = PartsType.Kit;
                    }
                    // 파츠가 아닌 경우
                    else
                    {
                        continue;
                    }
                    // 파츠 이미지 URL 추출
                    string src = item.FindElement(By.XPath("span[1]/a/img")).GetAttribute("src");
                    /*
                     * 아이템을 클릭해 설명, 효과 불러오기
                     * 설명, 효과를 동적으로 불러오기 때문에 클릭하지 않으면 출력되지 않음
                     */
                    item.Click();
                    // 파츠 효과
                    string effect = item.FindElement(By.XPath("div[1]/div[1]/div[2]/div[2]/span[2]")).Text;
                    // 파츠 설명
                    string description = item.FindElement(By.XPath("div[1]/div[1]/div[2]/div[2]/span[1]")).Text;
                    /*
                     * 파츠 이미지 예시: https://ssl.nx.com/s2/game/kart/kart2006/image/item2/KssS9QeC7W46_4.gif
                     * 파츠 키는 해당 이미지 URL의 ~kart2006/image/item2/KssS9QeC7W46_[4].gif에서 []임
                     */
                    int startIdx = src.LastIndexOf('_');
                    int endIdx = src.LastIndexOf('.');
                    string keyStr = src.Substring(startIdx + 1, endIdx - startIdx - 1);
                    int key = Convert.ToInt32(keyStr);
                    // 위에서 아이템 클릭해 나온 팝업창을 닫음
                    item.FindElement(By.XPath("div[1]/div[1]/div[1]/span/a")).Click();
                    result.Add(new Parts()
                    {
                        Description = description,
                        Effect = effect,
                        Key = key,
                        ImageUrl = src,
                        Name = name,
                        PartsType = partsType
                    });
                }
            }
        End:
            driver.Close();
            driver.Quit();
            return result;
        }
    }
}
