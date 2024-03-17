using Microsoft.EntityFrameworkCore.Migrations;
using System.Reflection;

namespace DataBase.Helpers
{
	internal static class MigrationBuilderHelper
	{
		public static void RunSqlFile(this MigrationBuilder builder, string filename)
		{
			var resFile = filename.GetSqlFile();

			builder.Sql(resFile);
		}

		internal static string GetSqlFile(this string filename)
		{
			var assembly = Assembly.GetExecutingAssembly();
			string resName = assembly.GetManifestResourceNames().Single(str => str.EndsWith(filename));

			using Stream stream = assembly.GetManifestResourceStream(resName);
			using StreamReader reader = new StreamReader(stream);

			return reader.ReadToEnd();
		}
	}
}
