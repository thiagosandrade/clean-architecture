// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Performance", "CA1861:Avoid constant arrays as arguments", Justification = "<Pending>", Scope = "member", Target = "~M:Infrastructure.Migrations.InitialDatabase.Up(Microsoft.EntityFrameworkCore.Migrations.MigrationBuilder)")]
[assembly: SuppressMessage("Major Vulnerability", "S2068:Credentials should not be hard-coded", Justification = "<Pending>", Scope = "member", Target = "~M:Infrastructure.Users.UserConfiguration.Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder{Domain.Users.User})")]
[assembly: SuppressMessage("Major Vulnerability", "S2068:Credentials should not be hard-coded", Justification = "<Pending>", Scope = "member", Target = "~M:Infrastructure.Persistence.Users.UserConfiguration.Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder{Domain.Users.User})")]
