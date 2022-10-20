using Firebase.Storage;
using NotesApp.MVVM.Model;
using NotesApp.MVVM.View;
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
using System.Windows;

namespace NotesApp.MVVM.ViewModel.Helpers
{
    public class DatabaseHelper
    {
        public static readonly HttpClient httpClient = new HttpClient();

        private static string dbFile = Path.Combine(Environment.CurrentDirectory, "Notes_DB.db3");
        private static string dbPath = "https://wpf-notes-app-c3ec4-default-rtdb.firebaseio.com/";

        public static async Task<bool> Insert<T>(T item)
        {
            string jsonBody = JsonSerializer.Serialize(item);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            
            var result = await httpClient.PostAsync($"{dbPath}{item.GetType().Name.ToLower()}.json", content);

            if (result.IsSuccessStatusCode)
                return true;
            else
                return false;
            
        }

        public static async Task<bool> Update<T>(T item) where T : IHasId
        {
            string jsonBody = JsonSerializer.Serialize(item);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            var result = await httpClient.PatchAsync($"{dbPath}{item.GetType().Name.ToLower()}/{item.Id}.json", content);
            
            if (result.IsSuccessStatusCode)
                return true;
            else
                return false;
            
        }

        public static async Task<bool> Delete<T>(T item) where T : IHasId
        {
            using (HttpClient client = new HttpClient())
            {
                if (item.GetType() == typeof(Note))
                    await new FirebaseStorage(NotesWindow.bucket).Child((item as Note).Id + ".rtf").DeleteAsync();

                var result = await httpClient.DeleteAsync($"{dbPath}{item.GetType().Name.ToLower()}/{item.Id}.json");

                if (result.IsSuccessStatusCode)
                    return true;
                else
                    return false;
            }
        }

        public static async Task<List<T>> Read<T>() where T : IHasId
        {
            try
            {
                var result = await httpClient.GetAsync($"{dbPath}{typeof(T).Name.ToLower()}.json");
                var jsonResult = await result.Content.ReadAsStringAsync();

                if (jsonResult != "null" && result.IsSuccessStatusCode)
                {
                    var objects = JsonSerializer.Deserialize<Dictionary<string, T>>(jsonResult);

                    List<T> list = new List<T>();
                    foreach (var o in objects)
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
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
            
        }
    }
}
