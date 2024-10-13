using IndexedDB.Blazor;
using Microsoft.JSInterop;
using System;

namespace JusGiveawayWebApp.Models
{
    public class JusGiveawayDB : IndexedDb
    {
        public JusGiveawayDB(IJSRuntime jSRuntime, string name, int version): base(jSRuntime, name, version) { }

        public IndexedSet<UserInfo> UserInfo { get; set; }
        public IndexedSet<UserGamePlayData> UserGameDatas { get; set; }

        public IndexedSet<GiveawayData> GiveawayDatas { get; set; }

        // Override the method to handle versioning and upgrading
        //protected override void OnUpgradeNeeded()
        //{
        //    // Check if the object store exists, if not create it
        //    if (!DbStoreNames.Contains("UserInfo"))
        //    {
        //        // Create a new object store (table) called UserInfo
        //        CreateStore<UserInfo>("UserInfo", store =>
        //        {
        //            // Setup the key path (e.g. primary key)
        //            store.SetKey(u => u.Id);
        //            // Optionally, define indexes (e.g. on email address, username)
        //            store.CreateIndex(u => u.UID, unique: true);
        //        });
        //    }

        //    // If you're upgrading, you can add new stores here based on the version
        //    // For instance, if you upgrade to version 2, you might add new object stores.
        //    if (DbVersion == 2)
        //    {
        //        // You can add new object stores or modify existing ones
        //    }
        //}

        public void ConfigureServices(IServiceCollection services)
        {
            // Create the database, set the version
            services.AddScoped(sp => new JusGiveawayDB(sp.GetService<IJSRuntime>(), "JusGiveawayDB", 2));  // Version 2
        }

    }

}
