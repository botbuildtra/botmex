[![Build Status](https://travis-ci.org/tperalta82/BitBotBackToTheFuture.svg?branch=master)](https://travis-ci.org/tperalta82/BitBotBackToTheFuture)

# BitBotBackToTheFuture
Bot of Bitmax

Example file "key.txt" (Configuration)

{

"key":"",

"secret":"",

"domain":"https://testnet.bitmex.com",

"pair":"XBTUSD",

"contract":300,

"profit":0.2,

"fee":0.075,

"long":"enable",

"short":"enable",

"interval":2000,

"intervalOrder":60000,

"timeGraph":"1m",


"webserver":"enable",

"webserverConfig":"http://localhost:5321/bot/",

"webserverIntervalCapture":300000,

"webserverKey":"",

"webserverSecret":"",


"indicatorsEntry":[

	{
		
"name":"CCI",

		"period":8
	},
	{
		
"name":"RSI",

		"period":8
	},
	{
		
"name":"BBANDS",

		"period":7
	}	
],



"indicatorsEntryCross":[


		{
			
"name":"MA",

			"period":5
		}


],

"indicatorsEntryDecision":[


	{
		
"name":"RSI",

		"period":8,
		"decision":"enable",
		"decisionPoint":40,
		"tendency":"enable"
	}
	

	]


}



