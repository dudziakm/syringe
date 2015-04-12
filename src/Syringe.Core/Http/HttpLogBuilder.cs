using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syringe.Core.Http
{
	public class HttpLogBuilder
	{
		// Examples
		public void LogRequest()
		{
//GET http://en.wikipedia.org/wiki/Microsoft HTTP/1.1
//Host: en.wikipedia.org
//Connection: keep-alive
//Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8
//User-Agent: Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2272.118 Safari/537.36
//Accept-Encoding: gzip, deflate, sdch
//Accept-Language: en-US,en;q=0.8
//Cookie: strategy2015_only5times_A=1; strategy2015_only5times_A-wait=0%7C0%7C1; GeoIP=GB:Wimbledon:51.4333:-0.2167:v4; uls-previous-languages=%5B%22en%22%5D; mediaWiki.user.sessionId=8a5caa4350d84752

		}

		public void AppendResponseHeaders()
		{
//HTTP/1.1 200 OK
//Server: Apache
//X-Content-Type-Options: nosniff
//X-Analytics: page_id=19001;ns=0
//Content-language: en
//X-UA-Compatible: IE=Edge
//Vary: Accept-Encoding,Cookie
//X-Powered-By: HHVM/3.3.1
//Last-Modified: Sun, 12 Apr 2015 14:11:21 GMT
//Content-Type: text/html; charset=UTF-8
//X-Varnish: 4107227022 4107212816, 661483373 661449298, 949501881 907135891
//Via: 1.1 varnish, 1.1 varnish, 1.1 varnish
//Content-Length: 425942
//Accept-Ranges: bytes
//Date: Sun, 12 Apr 2015 19:18:21 GMT
//Age: 18382
//Connection: keep-alive
//X-Cache: cp1065 hit (1), cp3041 hit (2), cp3030 frontend hit (46)
//Cache-Control: private, s-maxage=0, max-age=0, must-revalidate

//<!DOCTYPE html>
//<html lang="en" dir="ltr" class="client-nojs">

		}
	}
}
