using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using System.Net.Http.Json;
using Xunit;
using Xunit.Abstractions;
using NotePrivee.Models;

namespace NotePrivee.Test
{
    public class IntegrationAPITest
    {
        private readonly HttpClient client;
        private readonly TestServer server;
        private readonly notepriveeContext dbContext;

        private readonly ITestOutputHelper output;
        private readonly IWebHostBuilder builder;

        private readonly int NoteId;
        private readonly string NoteKey;
        private readonly string NoteContent;

        public IntegrationAPITest(ITestOutputHelper testOutputHelper)
        {
            builder = new WebHostBuilder().UseEnvironment("Testing").UseStartup<Startup>();
            server = new TestServer(builder);
            client = server.CreateClient();
            output = testOutputHelper;
            dbContext = server.Host.Services.GetService(typeof(notepriveeContext)) as notepriveeContext;
            NoteKey = Guid.NewGuid().ToString().Replace("-", string.Empty).Substring(0, 30);
            NoteContent = "note pour test intégration";
            Note nNotes = new Note { Contenu = SimpleAES.AES256.Encrypt(NoteContent, NoteKey) };
            dbContext.Notes.Add(nNotes);
            dbContext.SaveChanges();
            NoteId = nNotes.Id;
        }

        [Fact(DisplayName = "Création d'une note")]
        public async Task TestCreateNote()
        {
            Dictionary<String, String> jsonResult;
            Note note = new Note
            {
                Contenu = NoteContent
            };
            var response = await client.PostAsJsonAsync<Note>(@"/api/Notes", note);
            string result = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            jsonResult = JsonConvert.DeserializeObject<Dictionary<string, string>>(result);
            Assert.Equal(2, jsonResult.Count);
            Assert.NotNull(jsonResult["id"]);
            Assert.NotNull(jsonResult["key"]);

            output.WriteLine("id: " + jsonResult["id"]);
            output.WriteLine("key: " + jsonResult["key"]);
        }

        [Fact(DisplayName = "Récupération d'une note avec la mauvaise clé")]
        public async Task TestGetNoteByIdWrongKey()
        {
            var response = await client.GetAsync("/api/Notes?id=" + NoteId + "&key=123456");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact(DisplayName = "Récupération d'une note avec un ID érroné")]
        public async Task TestGetNoteByWrongId()
        {
            var response = await client.GetAsync("/api/Notes?id=123456&key=123456");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact(DisplayName = "Récupération d'une note")]
        public async Task TestGetNoteByIdKey()
        {
            Dictionary<String, String> jsonResult;
            var response = await client.GetAsync("/api/Notes?id=" + NoteId + "&key=" + NoteKey);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var result = await response.Content.ReadAsStringAsync();
            jsonResult = JsonConvert.DeserializeObject<Dictionary<string, string>>(result);

            Assert.Equal(jsonResult["contenu"], NoteContent);
        }
    }
}