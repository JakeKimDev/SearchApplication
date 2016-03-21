using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SearchApplication.Models
{
    public static class SearchJson
    {


        private static async Task<string> Request_Images_Json(String str_string)
        {
            // string url = "http://www.redmine.org/issues.json";

            string url =
             @"http://api.visitkorea.or.kr/openapi/service/rest/"+MainPage.SetLanguage+"/detailImage?" +
             @str_string;
            HttpClient client = new HttpClient();
            Task<string> getStringTask = client.GetStringAsync(url);
            string result = await getStringTask;


            return result;

        }
    }
}
