using NotesApp.MVVM.ViewModel.Interfaces;
using SQLite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NotesApp.MVVM.ViewModel.Helpers
{
    public class DatabaseHelper
    {
        private static string dbFile = Path.Combine(Environment.CurrentDirectory, "Notes_DB.db3");
        private static string dbPath = "https://wpf-notes-app-c3ec4-default-rtdb.firebaseio.com/";

        public static async Task<bool> Insert<T>(T item)
        {
            //bool result = false;
            //
            //using (SQLiteConnection conn = new SQLiteConnection(dbFile))
            //{
            //    conn.CreateTable<T>();
            //    int rows = conn.Insert(item);
            //    if (rows > 0)
            //        result = true;
            //}
            //
            //return result;

            string jsonBody = JsonSerializer.Serialize(item);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            using(HttpClient client = new HttpClient())
            {
                var result = await client.PostAsync($"{dbPath}{item.GetType().Name.ToLower()}.json", content);

                if (result.IsSuccessStatusCode)
                    return true;
                else
                    return false;
            }
        }

        public static async Task<bool> Update<T>(T item) where T : IHasId
        {
            //bool result = false;

            //using (SQLiteConnection conn = new SQLiteConnection(dbFile))
            //{
            //    conn.CreateTable<T>();
            //    int rows = conn.Update(item);

            //    if (rows > 0)
            //        return true;

            //}

            //return result;

            string jsonBody = JsonSerializer.Serialize(item);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            using (HttpClient client = new HttpClient())
            {
                var result = await client.PatchAsync($"{dbPath}{item.GetType().Name.ToLower()}/{item.Id}.json", content);

                if (result.IsSuccessStatusCode)
                    return true;
                else
                    return false;
            }
        }

        public static async Task<bool> Delete<T>(T item) where T : IHasId
        {
            //bool result = false;

            //using (SQLiteConnection conn = new SQLiteConnection(dbFile))
            //{
            //    conn.CreateTable<T>();
            //    int rows = conn.Delete(item);

            //    if (rows > 0)
            //        return true;

            //}

            //return result;

            using (HttpClient client = new HttpClient())
            {
                var result = await client.DeleteAsync($"{dbPath}{item.GetType().Name.ToLower()}/{item.Id}.json");

                if (result.IsSuccessStatusCode)
                    return true;
                else
                    return false;
            }
        }

        public static async Task<List<T>> Read<T>() where T : IHasId
        {
            //List<T> items;

            //using (SQLiteConnection conn = new SQLiteConnection(dbFile))
            //{
            //    conn.CreateTable<T>();
            //    items = conn.Table<T>().ToList();
            //}

            //return items;
            
            using (HttpClient client = new HttpClient())
            {
                var result = await client.GetAsync($"{dbPath}{typeof(T).Name.ToLower()}.json");
                var jsonResult = await result.Content.ReadAsStringAsync();

                if (jsonResult != "null" && result.IsSuccessStatusCode)
                {
                    var objects = JsonSerializer.Deserialize<Dictionary<string, T>>(jsonResult);

                    List<T> list = new List<T>();
                    foreach(var o in objects)
                    {
                        o.Value.Id = o.Key;
                        list.Add(o.Value);
                    }

                    return list;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
