namespace Syringe.Tests.Unit
{
	public class HttpTestData
	{
		public static string Basic()
		{
			return @"HTTP/1.1 200 OK 
Server: Apache
Cache-Control: private, s-maxage=0, max-age=0, must-revalidate

<html>
<body>
<input type=""hidden"" name=""__VIEWSTATE"" id=""__VIEWSTATE"" value=""IHqOZXmcw2RZXD/nR3+3RZlYLYRqbCgJVjIivE5c+BG5gIITC8IIsTxBMYgo7iE2Rx/aKOdm4Jv19PTZs+pAmoF96oBk10xOWIscaCkLhgTZcyc9B7MVjxU0HDE="" />


<input type=""hidden"" name=""__VIEWSTATE"" id=""__VIEWSTATE"" value=""IHqOZXmcw2RZXD/nR3+3RZlYLYRqbCgJVjIivE5c+BG5gIITC8IIsTxBMYgo7iE2Rx/aKOdm4Jv19PTZs+pAmoF96oBk10xOWIscaCkLhgTZcyc9B7MVjxU0HDE="" />
</body>
</html>
";
		}
	}
}