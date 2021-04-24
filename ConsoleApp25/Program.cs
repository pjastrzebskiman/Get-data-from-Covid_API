using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp25
{
    class Program
    {
        static void Main(string[] args)
        {

            var client = new RestClient("https://api.covid19api.com/countries");
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);
            //  Console.WriteLine(response.Content);
            List<Countries_list> list = JsonConvert.DeserializeObject<List<Countries_list>>(response.Content);
            List<Countries_list_detail> list_details_global = new List<Countries_list_detail>();

            List<Recovered> list_recovered = new List<Recovered>();
            List<Recovered> list_recovered_second = new List<Recovered>();
            List<Recovered> list_recovered_score_month = new List<Recovered>();

            for (int i = 0; i < list.Count; i++)
            {
                System.Threading.Thread.Sleep(1500);
                var client1 = new RestClient("https://api.covid19api.com/live/country/" + list[i].Country.ToString() + "/status/confirmed?from=2021-04-23T00:00:00Z&to=2021-04-23T00:00:01Z");
                var client2 = new RestClient("https://api.covid19api.com/live/country/" + list[i].Country.ToString() + "/status/confirmed?from=2021-03-23T00:00:00Z&to=2021-03-23T00:00:01Z");

                client1.Timeout = -1;
                client2.Timeout = -1;

                var request1 = new RestRequest(Method.GET);
                var request2 = new RestRequest(Method.GET);

                IRestResponse response1 = client1.Execute(request1);
                IRestResponse response2 = client2.Execute(request2);

                Console.WriteLine(response1.Content);

                List<Countries_list_detail> list_details = JsonConvert.DeserializeObject<List<Countries_list_detail>>(response1.Content);
                List<Countries_list_detail> list_details2 = JsonConvert.DeserializeObject<List<Countries_list_detail>>(response2.Content);

                Console.WriteLine(i);

                try
                {
                    // list_details_global.Add(list_details[0]);
                    list_recovered.Add(new Recovered()
                    {
                        Country = list[i].Country,
                        CounterRecovered = list_details[0].Recovered,
                        CounterConfirmed = list_details[0].Confirmed,
                        CounterDeaths = list_details[0].Deaths
                    });
                    list_recovered_second.Add(new Recovered()
                    {
                        Country = list[i].Country,
                        CounterRecovered = list_details2[0].Recovered,
                        CounterConfirmed = list_details2[0].Confirmed,
                        CounterDeaths = list_details2[0].Deaths
                    });
                }
                catch (ArgumentOutOfRangeException e)
                {
                    list_recovered.Add(new Recovered()
                    {
                        Country = list[i].Country,
                        CounterRecovered = 0,
                        CounterConfirmed = 0,
                        CounterDeaths = 0
                    });
                    list_recovered_second.Add(new Recovered()
                    {
                        Country = list[i].Country,
                        CounterRecovered = 0,
                        CounterConfirmed = 0,
                        CounterDeaths =0
                    });
                }
                Console.WriteLine(list[i].Country);
                try
                {
                    Console.WriteLine(list_details[0].Recovered);
                }
                catch
                {
                    Console.WriteLine("brak danych");
                }
            }

            for(int i=0;i<list_recovered.Count;i++)
            {
                list_recovered_score_month.Add(new Recovered()
                {
                    Country=list_recovered[i].Country,
                    CounterRecovered=list_recovered[i].CounterRecovered-list_recovered_second[i].CounterRecovered,
                    CounterConfirmed = list_recovered[i].CounterConfirmed - list_recovered_second[i].CounterConfirmed,
                    CounterDeaths = list_recovered[i].CounterDeaths - list_recovered_second[i].CounterDeaths
                });
            }
            var score =list_recovered_score_month.OrderBy(item => item.CounterRecovered);
            list_recovered_score_month = score.Reverse().ToList();
            string path = Path.Combine(Environment.CurrentDirectory, @"dane\", "wyniki_Recovered.txt");
            using (StreamWriter sciezkadoksiazki = new StreamWriter(path, false))
            {
                string listai = "";
                for (int i = 0; i < list_recovered_score_month.Count; i++)
                {
                    listai += list_recovered_score_month[i].Country +" "+ list_recovered_score_month[i].CounterRecovered+"\r\n";
                }
                sciezkadoksiazki.Write(listai);

            }

            Console.ReadLine();


        }
    }
}
