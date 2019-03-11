[![Build Status](https://travis-ci.org/tperalta82/botmex.svg?branch=master)](https://travis-ci.org/tperalta82/botmex)

# botmex
Bot of Bitmex

[Wiki do projeto]: https://github.com/tperalta82/botmex/wiki



Guia de configuração
A configuração do BOT é feita através do arquivo "key.json"

Antes de iniciar a configuração é necessário criar duas APIs na sua conta Bitmex:

API com permissão para criar ordens.
API sem nenhuma permissão especial.
Domain e WebsocketDomain
Estes campos devem ser alterados no caso de usar a Testnet. "domain":"https://www.bitmex.com", "websocketDomain":"wss://www.bitmex.com/realtime",

Chaves API
Configurar aqui a chave com permissão para criar ordens.

"key":"",

"secret":"",

Configurar aqui a chave da API sem permissão especial

"websocketKey":"",

"websocketSecret":"",

"apidebug":"false",

Configuração de banco de dados
Opção para uso futuro

"usedb":"disable",

"dbcon":"",

"dbquery":"",

Configuração do DashBoard
Dica :para acessar remotamente o DASHBOARD substitua aqui localhost pelo IP interno do computador.

"webserver":"enable", "webserverConfig":"http://localhost:5325/bot/",

"webserverIntervalCapture":100000,

Coniguração de valores de Trading
Moeda

"pair":"XBTUSD",

Quantidade de contratos

"contract":20,

Lucro esperado por operação em modo normal.

"profit":0.1,

Estimativa de taxa por operação

"fee":0.0001,

Opção Market Taker

Um Market Taker concorda com os preços atualmente listados no "Order Book" e deseja preencher sua ordem imediatamente.

"marketTaker":"disable",

Numero maximo de ordens

"limiteOrder":3,

Valor do degrau na operação scalping, (em USD)

"stepvalue":4,

Coniguração de controle de Risco
Valor do stoploss em % do peço da operação

"stoploss":0.5,

Tipo de stoploss opçoes : Orderbook , Bot

"stoplosstype":"orderbook", "stoplossInterval":6,

"stopgain":10,

Coniguração de Operação
Tipo de operação: scalper, scalperv2, normal, surf

"operation":"scalperv2",

"obDiff":0.5,

"tendencyBook":"disable",

Habilitar / Desabilitar Long e ou Short

"long":"enable",

"short":"enable",

"roe":"automatic",

"interval":14500,

"intervalOrder":50000,

"timeGraph":"1m",

"intervalCancelOrder":200,

Configuração de Estratégias e Indicadores
"carolatr":"enable",

"atrvalue":50,

"strategyOptions":[

​ { ​ "name":"invert",

​ "value":"true"

​ },

​ { "name":"invertmultiplier",

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
] }


```




```