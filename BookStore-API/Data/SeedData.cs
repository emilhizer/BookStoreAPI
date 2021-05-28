using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace BookStore_API.Data {

  // Class to create initial username / password for demo
  public static class SeedData {

    //public static void Seed(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager) {
    public async static Task Seed(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager) {
      await SeedRoles(roleManager);
      await SeedUsers(userManager);
    } // Seed


    //private static void SeedRoles(RoleManager<IdentityRole> roleManager) { // this is a synchronous method
    private async static Task SeedRoles(RoleManager<IdentityRole> roleManager) { // made this an async method
      //if (!roleManager.RoleExistsAsync("Administrator").Result) { // this is NOT an async call by adding .Result to the end, will refactor to be async later
      if (!await roleManager.RoleExistsAsync("Administrator")) { // this is NOT an async call by adding .Result to the end, will refactor to be async later
          var role = new IdentityRole {
          Name = "Administrator"
        };
        //var result = roleManager.CreateAsync(role).Result;
        await roleManager.CreateAsync(role);
      }
      if (!await roleManager.RoleExistsAsync("Customer")) {
        var role = new IdentityRole {
          Name = "Customer"
        };
        await roleManager.CreateAsync(role);
      }
    } // SeedRoles


    private async static Task SeedUsers(UserManager<IdentityUser> userManager) {
      // Seed the admin
      if(await userManager.FindByEmailAsync("admin@bookstore.com") == null) {
        var user = new IdentityUser {
          UserName = "admin",
          Email = "admin@bookstore.com"
        };
        var result = await userManager.CreateAsync(user, "P@ssword1");
        if (result.Succeeded) {
          await userManager.AddToRoleAsync(user, "Administrator");
        }
      } // Seed the admin
      // Seed a customer
      if (await userManager.FindByEmailAsync("customer1@gmail.com") == null) {
        var user = new IdentityUser {
          UserName = "customer1",
          Email = "customer1@gmail.com"
        };
        var result = await userManager.CreateAsync(user, "P@ssword1");
        if (result.Succeeded) {
          await userManager.AddToRoleAsync(user, "Customer");
        }
      } // Seed a customer
      // Seed another customer
      if (await userManager.FindByEmailAsync("customer2@gmail.com") == null) {
        var user2 = new IdentityUser {
          UserName = "customer2",
          Email = "customer2@gmail.com"
        };
        var result2 = await userManager.CreateAsync(user2, "P@ssword1");
        if (result2.Succeeded) {
          await userManager.AddToRoleAsync(user2, "Customer");
        }
      } // Seed another customer
    } // SeedUsers


  } // SeedDate
} // namespace
