[![Build Status](https://travis-ci.org/tperalta82/botmex.svg?branch=master)](https://travis-ci.org/tperalta82/botmex)

# botmex
Bot of Bitmex

Example file "key.json" (Configuration)
```json
{
   "key":"",
   "secret":"",
   "domain":"https://www.bitmex.com",
   "websocketDomain":"wss://www.bitmex.com/realtime",
   "websocketKey":"",
   "websocketSecret":"",
   "apidebug":"false",
   "usedb":"disable",
   "dbcon":"",
   "dbquery":"",
   "pair":"XBTUSD",
   "contract":20,
   "profit":0.1,
   "fee":0.0001,
   "marketTaker":"disable",
   "limiteOrder":3,
   "stoploss":0.5,
   "stoplosstype":"orderbook",
   "stoplossInterval":6,
   "stepvalue":4,
   "stopgain":10,
   "operation":"scalperv2",
   "obDiff":0.5,
   "tendencyBook":"disable",
   "long":"enable",
   "short":"enable",
   "roe":"automatic",
   "interval":14500,
   "intervalOrder":50000,
   "timeGraph":"1m",
   "intervalCancelOrder":200,
   "webserver":"enable",
   "webserverConfig":"http://localhost:5325/bot/",
   "webserverIntervalCapture":100000,
   "carolatr":"enable",
   "atrvalue":50,
   "strategyOptions":[
      {
         "name":"invert",
         "value":"true"
      },
      {
         "name":"invertmultiplier",
         "value":"2"
      },
      {
         "name": "invertmarket",
         "value":"false"
      }
   ],
   "indicatorsEntry":[
      {
         "name":"RSI",
         "period":9,
         "high":70,
         "low":30
      },
      {
         "name":"MFI",
         "period":20,
         "high":70,
         "low":30
      },
      {
         "name":"CCI",
         "period":20,
         "high":160,
         "low":-160
      },
      {
         "name":"MATENDENCY",
         "long":185,
         "short":130
      }
   ],
   "indicatorsEntryCross":[

   ],
   "indicatorsEntryDecision":[

   ],
   "indicatorsEntryThreshold":[
      {
         "name":"ATRD",
         "period":7,
         "limit":50
      }
   ],
   "indicatorsInvert":[
      {
         "name":"MATENDENCY",
         "high":185,
         "low":130
      }
   ]
}
```



