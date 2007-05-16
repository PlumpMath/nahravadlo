Napoveda k programu Nahravadlo 0.1.3 by Arcao
--------------------------------------------------------------------------------

Aplikace slouzi k nahravani kolejni TV za pomoci VLC, kde v programu napisete
nazev nahravani, vyberete stanici, zadate datum a cas zacatku, vzpnite delku
nahravani a vyberete soubor, kam se ma ukladat. Zmacknutim tlacitka Pridat,
dojde k vytvoreni Naplanovane ulohy ve Windows XP. 

Instalace je jednoducha. Staci mit nainstalovan .NET 2.0 framework. Aplikaci
neni nutne jinak instalovat, staci ji jednoduse spoustet, ALE NEJPRVE
NAKONFIGUROVAT, VIZ DALE.

Poznamka: 
   Od verze 0.1.3 Nahravadlo vytvari samo config.xml a soubor TaskScheduler.dll
   je zapouzdren v Aplikaci.

------------
KONFIGURACE:
------------
Konfigurace se spusti automaticky pri startu programu, pokud nebude nalezen 
config.xml, nebu rucne pomoci polozky "Nastaveni" v okne vpravo nahore.

Pro spravnou funkci programu je nutne nastavit:

 1) Nastaveni cesty vcetne souboru, kde se nachazi VLC (nejlepe posledni verze)

 2) Vychozi adresar, kam se bude ukladat soubor, pokud nebude uveden soubor 
    s plnou cestou pri zakladani nahravani.

 3) Uprava uzivatele, pod kterym bude spusteno VLC. Pokud ma byt nahravani 
    spusteno na pozadi, tak aby nebylo videt okno VLC, pak je nutne zadat jmeno
    a heslo existujiciho uzivatele ve Windows XP (heslo nesmi byt prazdne!).
    Pokud nevyplnime uzivatele a heslo, veme se jako uzivatel prave prihlaseny
    uzivatel ve Windows XP. V tomto pripade pak po spusteni nahravani bude videt
    okno aplikace VLC (nahravani se neprovede, pokud uzivatel nebude prihlasen!).

    Poznamka: Uzivatelske jmeno je mozne zadavat vcetne domeny. Uzivatel, pod
              kterym bude spusteno VLC musi mit moznost tuto aplikaci spustit a
              musi mit moznost pristupovat i k prostoru na disku, kam se bude
              ukladat soubor.

  4) Nastaveni jestli pouzit MPEG TS nebo MPEG PS kontejner vznikl z duvodu, ze 
     VLC ve starsich verzich (0.7.x) ma problemy s ukladanim do MPEG PS, takze
     pokud stale pouzivate VLC 0.7.x, je vhodne zaskrtnou tuto volbu.

     Poznamka: Konkretne chyba se projevuje v tom, že se nelze posouvat
               ve videu, pripadne pri posunuti prehravac spadne - priklad
               Windows Media Playeru. Proto doporucuji pouzit radeji posledni
               verzi VLC a nechat tuto volbu odskrtnutou, tzn pouzit MPEG PS
               kontejner.

--------------------------------------------------------------------------------

Historie: (o = oprava chyby, + = pridana vlastnost)

 v 0.1.3
    + Pridan konfiguracni dialog, nyni jiz neni nutne sahat do config.xml rucne
    + Moznost nastavit, zda nahravat do MPEG PS nebo MPEG TS kontejneru
    o Ve vychozim nastaveni aplikace uklada do MPEG PS, cimz je mozne vysledny
      MPEG soubor nacist v jakem koliv software pro prehravani videa
    o Opraveny ruzne pady aplikace, u vybraneho nahravani zobrazuje stav 
      v realnem case
    o Nyni by se melo jiz vypnout VLC, pri mazani spusteneho nahravani

 v 0.1.2 
    o Delka poradu sla nastavit jen 100 minut, maximum je nyni 999 minut.
    + Moznost si nastavit v config.xml adresar, kam se budou ukladat soubory,
      pokud nebudou vyplnene vcetne cesty.

--------------------------------------------------------------------------------

FAQ:

 Q: Je mozne zmenit nazev nahravani?
 A: Ne v primo v aplikaci, jedina moznost je ho zmenit v Naplanovanych ulohach
    (Ovladaci panely -> Naplanovane ulohy). Nezapomente ze nazev naplanovane
    ulohy musi zacinat textem "Nahravani - " (s diakritikou), jinak naplanovanou
    ulohu aplikace Nahravadlo nezobrazi.

 Q: Je mozne smazat a zastavit jiz spustenou ulohu?
 A: Smazat ano, ale jiz se aplikace nevypne (vypne se az po uplynuti nastavene
    delky programu). Pokud by jste ji presto chteli vypnout, tak bud pres
    Naplanovane ulohy (v Ovladacich panelech) nebo pres Spravce uloh (podproces 
    svchost.exe pokud vidite strom procesu - napr v Process Explorer).

    Od verze 0.1.3 by se jiz aplikace mela vypnout.

 Q: Pouzivame jine udp adresy pro streamovani TV, pripadne mam vice TV, nez je
    vypsano. Co s tim?
 A: Jednoducha naprava. Staci upravit adresy v config.xml v sekci
    nahravadlo/programy. Pripadne je mozne ubrat i pridat programy vymazanim
    patricne vetve z nahravadlo/programy v config.xml.
    
    Od verze 0.1.3 je nejprve nutne spustit Nahravadlo, aby se config.xml
    vytvoril, pokud neexistuje.

 Q: Co kdyz nemam Naplanovane ulohy? (Windows osekan pomoci nLite)
 A: V tom pripade mate smulu.

 Q: Jaky je rozdil mezi MPEG TS a MPEG PS?
 A: MPEG TS (Transport Stream) se pouziva pro prenaseni MPEG zaznamu v prostredi,
    kde mohou vznikat chyby, napr: DVB-T, streamovani po siti, atp., protoze 
    v sobe obsahuje moznost opravovani chyb. Narozdil od toho se MPEG PS 
    (Program Stream) vyuziva v prostredi, kde opravu chyb dokaze zajistit jina
    technologie, napr. DVD, video na disku, atp.

 Q: V cem prehraji soubory s MPEG TS a MPEG PS kontejnerem?
 A: MPEG PS prehraje jakykoliv software pro sledovani videa. Pro prehrani 
    MPEG TS kontejneru, je jiz potrebny vetsinou nejaky plugin. Pro programy
    pouzivajici DirectShow, jako napriklad Windows Media Player, BSPlayer,
    MV2Player a jine lze použít Haali Media Splitter. Prehrati MPEG TS bez
    instalace pluginu umi treba VLC, nebo preportovany linuxovy MPlayer 
    na Windows.

    Haali Media Splitter najdete na: http://haali.cs.msu.ru/mkv/ (odkaz na 
    stahnuti je vpavo nahore).

 Q: Co te vedlo k napsani aplikace?
 A: Mit moznost si dopredu nastavit nahravani, pac jsem clovek zapominavy. :o)

 Q: Jak je to s siritelnosti a zdrojovymi kody?
 A: Sirte dle libosti, jen bacha na hesla v config.xml, aby ste to nesirili se
    svym Orion loginem a heslem. Zdrojaky lze vsemozne upravovat, jen prosim
    pokud se do toho vrhnete, nezapomente se o me zminit v Aboutu. Jinak je to
    naprogramovany v C# v prostredi Visual Studia .NET 2005 SP1 na .NET 2.0 
    frameworku.

--------------------------------------------------------------------------------

Kontakt: 

  Martin Arcao Sloup
  e-mail: arcao@arcao.com
  jabber: arcao@jabber.cz
  icq#:   128-921-427
  www:    http://arcao.com

  Kdyby jste nesehnali zdrojaky muzete si o ne napsat, rad vam je zaslu.

  Nove verze programu by se meli objevit na http://files.arcao.com/nahravadlo/

--------------------------------------------------------------------------------
EOF.    