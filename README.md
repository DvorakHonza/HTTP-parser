# HTTP-parser

Tento projekt obsahuje parser, který pomocí knihovny [Pidgin](https://github.com/benjamin-hodgson/Pidgin) zpracovává zprávy aplikačního protokolu HTTP. Parser se řídí formátem zpráv HTTP protokolu specifikovaném v dokumentech [RFC 7230](https://tools.ietf.org/html/rfc7230) a [RFC 7231](https://tools.ietf.org/html/rfc7231) a dodatečnými pravidly ABNF popsanými v [RFC 3986](https://tools.ietf.org/html/rfc3986).

## Reprezentace HTTP zprávy

Zprává HTTP protokolu se skládá ze stavového řádku (Startline), hlaviček a těla, oddělených prázdným řádkem (CRLF). Tělo zprávy je posloupnost bytů, jejíž sémantika je dána informacemi (header fields) uvedenými v hlavičkách. HTTP rozlišuje dva typy zpráv a sice Request a Response. Stavový řádek určuje, zda se jedná o Request, v takovém případě je Startline typu Requestline nebo o Response, kdy se jedná o Statusline. Hlavičky obsahují informace o klientovy, serveru, typu přenášených dat a jiné.

Výše zmíněná struktura je implementována pomocí tříd a závislostmi mezi nimi, jako je znázorněno v následujícím grafu.

<div class=mermaid>
  graph TD;
    mess[HttpMessage]-->head[HttpHeader];
    mess-->body[HttpBody];
    head-->sLine[StartLine];
    head[HttpHeader]-->hFields[HeaderFields];
    sLine-->rLine[RequestLine];
    sLine-->statL[StatusLine];
    rLine-->Method;
    rLine-->rTarget[RequestTarget];
    rLine-->Version;
    statL-->Version;
    statL-->StatusCode;
    statL-->ReasonPhrase;
    rTarget-->OriginForm;
    rTarget-->AbsoluteForm;
    rTarget-->AsteriskForm;
    rTarget-->AuthorityForm;
</div>


## Testovací dataset

Testovací data ve formě Packet Capture souborů jsou uložena v adresáři Dataset, který se nachází v kořenovém adresáři. Konkrétní streamy uloženy ve formě bin souborů jsou uloženy v adresáři ParserTests/Resources.

Složky Requests a Responses obsahují samostatné zprávy, které sloužily k testování jednotlivých parserů v průběhu implementace. Složka Streams obsahuje celé streamy HTTP zpráv, konkrétní bin soubory a příslušné pcap soubory jsou uvedeny v následujícím seznamu.

* http_tcp_stream_0.pcap - obsahuje jednoduchý požadavek a odpověď obsahující webovou stránku (http_tcp_stream_0.bin)

* http_tcp_stream_1.pcap - obsahuje jednoduchý požadavek a odpověď s několika query (http_tcp_stream_1.bin)

* http_gzip.pcap - obsahuje požadavek a odpověď obsahující soubor komprimovaný pomocí gzip (http_gzip.bin)

* http_head.pcap - obsahuje požadavek s metodou HEAD a odpověď (http_head.bin)

* http_proxy_connection.pcapng - obsahuje žádost s připojení k proxy pomocí metody CONNECT a odpověď (http_proxy_connection.bin)

* http_jpeg.pcap - obsahuje žádost a odpověď s JPEG obrázkem

## Nedostatky implementace

* Nelze určit, zda zpráva obsahuje tělo, pokud je tato informace závislá na typu požadavku.

* Pokud je Transfer-Encoding nastaveno na chunked, tak tělo zpracuje pouze v znovu sestavené podobě
