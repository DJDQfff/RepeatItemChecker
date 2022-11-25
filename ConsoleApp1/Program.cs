// See https://aka.ms/new-console-template for more information
using FolderGroupDB;

using Microsoft.EntityFrameworkCore;

Console.WriteLine("Hello, World!");

using (Database_1 database_1 = new Database_1())
{
    database_1.Database.Migrate();
    FoldersGroup foldersGroup = new FoldersGroup("1");

    foldersGroup.AddToken("jdfaj");
    foldersGroup.AddToken("fjdkaln");
    var a = database_1.Add(foldersGroup);
    database_1.SaveChanges();
    a.Entity.RemoveToken("jdfaj");
    database_1.SaveChanges();
}