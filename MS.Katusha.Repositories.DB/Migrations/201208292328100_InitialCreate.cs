namespace MS.Katusha.Repositories.DB.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Profiles",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserId = c.Long(nullable: false),
                        Name = c.String(maxLength: 64),
                        Location_CityCode = c.Int(nullable: false),
                        Location_CityName = c.String(maxLength: 200),
                        Location_CountryCode = c.String(maxLength: 2),
                        Location_CountryName = c.String(maxLength: 200),
                        BodyBuild = c.Byte(nullable: false),
                        EyeColor = c.Byte(nullable: false),
                        HairColor = c.Byte(nullable: false),
                        Smokes = c.Byte(nullable: false),
                        Alcohol = c.Byte(nullable: false),
                        Religion = c.Byte(nullable: false),
                        Gender = c.Byte(nullable: false),
                        DickSize = c.Byte(nullable: false),
                        DickThickness = c.Byte(nullable: false),
                        BreastSize = c.Byte(nullable: false),
                        Height = c.Int(nullable: false),
                        BirthYear = c.Int(nullable: false),
                        Description = c.String(maxLength: 4000),
                        ProfilePhotoGuid = c.Guid(nullable: false),
                        FriendlyName = c.String(maxLength: 64),
                        Guid = c.Guid(nullable: false),
                        ModifiedDate = c.DateTime(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        DeletionDate = c.DateTime(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Password = c.String(nullable: false, maxLength: 14),
                        FacebookUid = c.String(),
                        PaypalPayerId = c.String(),
                        Gender = c.Byte(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 64),
                        Email = c.String(nullable: false, maxLength: 64),
                        EmailValidated = c.Boolean(nullable: false),
                        Phone = c.String(),
                        UserRole = c.Long(nullable: false),
                        Expires = c.DateTime(nullable: false),
                        MembershipType = c.Byte(nullable: false),
                        Guid = c.Guid(nullable: false),
                        ModifiedDate = c.DateTime(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        DeletionDate = c.DateTime(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SearchingFors",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ProfileId = c.Long(nullable: false),
                        Search = c.Byte(nullable: false),
                        ModifiedDate = c.DateTime(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        DeletionDate = c.DateTime(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Profiles", t => t.ProfileId, cascadeDelete: true)
                .Index(t => t.ProfileId);
            
            CreateTable(
                "dbo.CountriesToVisits",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ProfileId = c.Long(nullable: false),
                        Country = c.String(maxLength: 2),
                        ModifiedDate = c.DateTime(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        DeletionDate = c.DateTime(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Profiles", t => t.ProfileId, cascadeDelete: true)
                .Index(t => t.ProfileId);
            
            CreateTable(
                "dbo.LanguagesSpokens",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ProfileId = c.Long(nullable: false),
                        Language = c.String(maxLength: 2),
                        ModifiedDate = c.DateTime(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        DeletionDate = c.DateTime(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Profiles", t => t.ProfileId, cascadeDelete: true)
                .Index(t => t.ProfileId);
            
            CreateTable(
                "dbo.Photos",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ProfileId = c.Long(nullable: false),
                        Description = c.String(),
                        ContentType = c.String(),
                        FileName = c.String(),
                        Status = c.Byte(nullable: false),
                        Guid = c.Guid(nullable: false),
                        ModifiedDate = c.DateTime(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        DeletionDate = c.DateTime(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Profiles", t => t.ProfileId, cascadeDelete: true)
                .Index(t => t.ProfileId);
            
            CreateTable(
                "dbo.Conversations",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        FromId = c.Long(nullable: false),
                        ToId = c.Long(nullable: false),
                        Message = c.String(nullable: false, maxLength: 4000),
                        Subject = c.String(maxLength: 255),
                        ReadDate = c.DateTime(nullable: false),
                        Guid = c.Guid(nullable: false),
                        ModifiedDate = c.DateTime(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        DeletionDate = c.DateTime(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Profiles", t => t.FromId)
                .ForeignKey("dbo.Profiles", t => t.ToId)
                .Index(t => t.FromId)
                .Index(t => t.ToId);
            
            CreateTable(
                "dbo.Visits",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ProfileId = c.Long(nullable: false),
                        VisitorProfileId = c.Long(nullable: false),
                        VisitCount = c.Int(nullable: false),
                        ModifiedDate = c.DateTime(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        DeletionDate = c.DateTime(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Profiles", t => t.VisitorProfileId)
                .ForeignKey("dbo.Profiles", t => t.ProfileId)
                .Index(t => t.VisitorProfileId)
                .Index(t => t.ProfileId);
            
            CreateTable(
                "dbo.States",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ProfileId = c.Long(nullable: false),
                        Gender = c.Byte(nullable: false),
                        LastOnline = c.DateTime(nullable: false),
                        BirthYear = c.Int(nullable: false),
                        Height = c.Int(nullable: false),
                        CountryCode = c.String(),
                        CityCode = c.Int(nullable: false),
                        BodyBuild = c.Byte(nullable: false),
                        HairColor = c.Byte(nullable: false),
                        EyeColor = c.Byte(nullable: false),
                        CountriesToVisit = c.String(),
                        Searches = c.String(),
                        HasPhoto = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ConfigurationDatas",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Key = c.String(),
                        Value = c.String(),
                        ModifiedDate = c.DateTime(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        DeletionDate = c.DateTime(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Resources",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ResourceKey = c.String(),
                        Value = c.String(),
                        Language = c.String(maxLength: 2),
                        ModifiedDate = c.DateTime(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        DeletionDate = c.DateTime(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ResourceLookups",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ResourceKey = c.String(),
                        Value = c.String(),
                        Language = c.String(maxLength: 2),
                        LookupName = c.String(),
                        Order = c.Byte(nullable: false),
                        LookupValue = c.Byte(nullable: false),
                        ModifiedDate = c.DateTime(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        DeletionDate = c.DateTime(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PhotoBackups",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Data = c.Binary(),
                        Guid = c.Guid(nullable: false),
                        ModifiedDate = c.DateTime(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        DeletionDate = c.DateTime(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.GeoCountries",
                c => new
                    {
                        ISO = c.String(nullable: false, maxLength: 128),
                        ISO3 = c.String(),
                        ISONumeric = c.Int(nullable: false),
                        FIPS = c.String(),
                        Country = c.String(),
                        Capital = c.String(),
                        Area = c.Int(nullable: false),
                        Population = c.Long(nullable: false),
                        Continent = c.String(),
                        TLD = c.String(),
                        CurrencyCode = c.String(),
                        CurrencyName = c.String(),
                        Phone = c.String(),
                        PostalCodeFormat = c.String(),
                        PostalCodeRegEx = c.String(),
                        Languages = c.String(),
                        GeoNameId = c.Int(nullable: false),
                        Neighbors = c.String(),
                        EquivalentFipsCode = c.String(),
                    })
                .PrimaryKey(t => t.ISO);
            
            CreateTable(
                "dbo.GeoLanguages",
                c => new
                    {
                        LanguageName = c.String(nullable: false, maxLength: 128),
                        ISO639_3 = c.String(),
                        ISO639_2 = c.String(),
                        ISO639_1 = c.String(),
                    })
                .PrimaryKey(t => t.LanguageName);
            
            CreateTable(
                "dbo.GeoNames",
                c => new
                    {
                        GeoNameId = c.Int(nullable: false),
                        Name = c.String(maxLength: 200),
                        AsciiName = c.String(maxLength: 200),
                        AlternateNames = c.String(),
                        Latitude = c.Double(nullable: false),
                        Longitude = c.Double(nullable: false),
                        FeatureClass = c.String(maxLength: 1),
                        FeatureCode = c.String(maxLength: 10),
                        CountryCode = c.String(maxLength: 2),
                        CC2 = c.String(maxLength: 60),
                        Admin1code = c.String(maxLength: 20),
                        Admin2code = c.String(maxLength: 80),
                        Admin3code = c.String(maxLength: 20),
                        Admin4code = c.String(maxLength: 20),
                        Population = c.Long(nullable: false),
                        Elevation = c.Int(nullable: false),
                        DEM = c.String(),
                        TimeZone = c.String(maxLength: 40),
                        ModificationDate = c.String(),
                    })
                .PrimaryKey(t => t.GeoNameId);
            
            CreateTable(
                "dbo.GeoTimeZones",
                c => new
                    {
                        TimeZoneId = c.String(nullable: false, maxLength: 128),
                        GMTOffset = c.Double(nullable: false),
                        DSTOffset = c.Double(nullable: false),
                        RawOffset = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.TimeZoneId);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.Visits", new[] { "ProfileId" });
            DropIndex("dbo.Visits", new[] { "VisitorProfileId" });
            DropIndex("dbo.Conversations", new[] { "ToId" });
            DropIndex("dbo.Conversations", new[] { "FromId" });
            DropIndex("dbo.Photos", new[] { "ProfileId" });
            DropIndex("dbo.LanguagesSpokens", new[] { "ProfileId" });
            DropIndex("dbo.CountriesToVisits", new[] { "ProfileId" });
            DropIndex("dbo.SearchingFors", new[] { "ProfileId" });
            DropIndex("dbo.Profiles", new[] { "UserId" });
            DropForeignKey("dbo.Visits", "ProfileId", "dbo.Profiles");
            DropForeignKey("dbo.Visits", "VisitorProfileId", "dbo.Profiles");
            DropForeignKey("dbo.Conversations", "ToId", "dbo.Profiles");
            DropForeignKey("dbo.Conversations", "FromId", "dbo.Profiles");
            DropForeignKey("dbo.Photos", "ProfileId", "dbo.Profiles");
            DropForeignKey("dbo.LanguagesSpokens", "ProfileId", "dbo.Profiles");
            DropForeignKey("dbo.CountriesToVisits", "ProfileId", "dbo.Profiles");
            DropForeignKey("dbo.SearchingFors", "ProfileId", "dbo.Profiles");
            DropForeignKey("dbo.Profiles", "UserId", "dbo.Users");
            DropTable("dbo.GeoTimeZones");
            DropTable("dbo.GeoNames");
            DropTable("dbo.GeoLanguages");
            DropTable("dbo.GeoCountries");
            DropTable("dbo.PhotoBackups");
            DropTable("dbo.ResourceLookups");
            DropTable("dbo.Resources");
            DropTable("dbo.ConfigurationDatas");
            DropTable("dbo.States");
            DropTable("dbo.Visits");
            DropTable("dbo.Conversations");
            DropTable("dbo.Photos");
            DropTable("dbo.LanguagesSpokens");
            DropTable("dbo.CountriesToVisits");
            DropTable("dbo.SearchingFors");
            DropTable("dbo.Users");
            DropTable("dbo.Profiles");
        }
    }
}
