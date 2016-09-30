# Nahrávadlo
Aplikace slouží k nahrávání kolejní TV za pomocí **VLC**, kde v programu napíšete název nahrávání, vyberete stanici, zadáte datum a čas začátku, vyplníte délku nahrávání a vyberete soubor, kam se má ukládat. Zmáčknutím tlačítka Přidat, dojde k vytvoření **Naplánované úlohy ve Windows**. 
Instalace je jednoduchá. Stačí mít nainstalován .NET 2.0 framework. Aplikaci není nutné jinak instalovat, stačí ji jednoduše spouštět, **ale nejprve je ji potřeba nakonfigurovat, viz dále**.

*Poznámka: Od verze 0.1.3 Nahrávadlo vytváří samo `config.xml` a soubor `TaskScheduler.dll` je zapouzdřen v Aplikaci.*

## Konfigurace
Konfigurace se spustí automaticky při startu programu, pokud nebude nalezen config.xml, nebu ručně pomoci položky **Nastavení** v okně vpravo nahoře.

Pro správnou funkci programu je nutné nastavit:
* Nastavení cesty včetně souboru, kde se nachází **VLC** (nejlépe poslední verze)
* Výchozí adresář, kam se bude ukládat soubor, pokud nebude uveden soubor s plnou cestou při zakládání nahrávání.
* Úprava uživatele, pod kterým bude spuštěno VLC. Pokud má být nahrávání spuštěno na pozadí tak, aby nebylo vidět okno VLC, pak je nutné zadat jméno a heslo existujícího uživatele ve Windows (heslo nesmí být prázdné!).<br /><br />
  Pokud není vyplněn uživatel a hesl, vezme se jako uživatel právě přihlášeny uživatel ve Windows. V tomto případě se pak po spuštění nahrávání zobrazí okno aplikace VLC (nahrávání se neprovede, pokud uživatel nebude přihlášen!).<br /><br />
  Poznámka: Uživatelské jméno je možné zadávat včetně domény. Uživatel, pod kterým bude spuštěno **VLC** musí mít možnost tuto aplikaci spustit a musí mít možnost přistupovat i k prostoru na disku, kam se bude ukládat soubor.
* Nastavení jestli použít MPEG TS nebo MPEG PS kontejner vznikl z důvodu, že **VLC** ve starších verzích (0.7.x) má problémy s ukládáním do MPEG PS, takže pokud stále používáte VLC 0.7.x, je vhodné zaškrtnou tuto volbu.<br /><br />
  Poznámka: Konkrétně chyba se projevuje v tom, že se nelze posouvat ve videu, případně při posunutí přehrávač spadne - příklad Windows Média Playeru. Proto doporučují použít raději poslední verzi VLC a nechat tuto volbu odškrtnutou, tzn. použít MPEG PS kontejner.
* Od verze 0.1.5 je možné v Nastavení navolit kanály/stanice, které program má používat. Včetně importu/exportu kanálu/stanic z/do souboru ve formátu XSPF.

## Historie

### v 0.1.5
* Přidaná správa kanálu/stanic v nastavení včetně importu a exportu z/do formátu XSPF
* Opraveno pár chyb v podvozku aplikace

### v 0.1.4
* Přidaná možnost okamžitě spustit nahrávání pod aktuálně přihlášeným uživatelem.
* Nyní je možné nastavit délku nahrávání i z datumu a času konce nahrávání
* Opraveno: Po uložení a zavření okna Nastavení se seznam stanic naplnil duplicitními položkami

### v 0.1.3
* Přidán konfigurační dialog, nyní již není nutné sahat do config.xml ručně
* Možnost nastavit, zda nahrávat do MPEG PS nebo MPEG TS kontejnerů
* Ve výchozím nastavení aplikace ukládá do MPEG PS, čímž je možné výsledný MPEG soubor načíst v jakém koliv software pro přehrávání videa
* Opraveny různé pády aplikace, u vybraného nahrávání zobrazuje stav v reálném čase
* Nyní by se mělo již vypnout VLC, při mazání spuštěného nahrávání

### v 0.1.2
* Opraveno: Délka pořadu šla nastavit jen 100 minut, maximum je nyní 999 minut.
* Možnost si nastavit v config.xml adresář, kam se budou ukládat soubory, pokud nebudou vyplněné včetně cesty.

## FAQ
**Q:** Je možné změnit název nahrávání?<br />
**A:** Ne v přímo v aplikaci, jediná možnost je ho změnit v **Naplánovaných úlohách** */Ovládací panely -> Naplánované úlohy/*. Nezapomeňte že název naplánované úlohy musí začínat textem `Nahrávání - `, jinak naplánovanou úlohu aplikace Nahrávadlo nezobrazí.

**Q:** Je možné smazat a zastavit již spuštěnou úlohu?<br />
**A:** Smazat ano, ale již se aplikace nevypne (vypne se až po uplynutí nastavené délky programu). Pokud by jste ji přesto chtěli vypnout, tak buď přes **Naplánované úlohy (v Ovládacích panelech)** nebo přes **Správce úloh (je to podproces 
svchost.exe pokud vidíte strom procesu - např. v Process Explorer)**.<br />
<br />
Od verze 0.1.3 by se již aplikace měla vypnout.

**Q:** Používáme jiné udp adresy pro streamování TV, případně mám více TV, než je vypsáno. Co s tím?<br />
**A:** Od verze 0.1.5 je možné kanály spravovat v nastavení programu. Pro hromadnou distribuci programu pro lidí, kteří mají stejné nastavení doporučuji, buď společně s aplikací dodávat nastavení kanálu/stanic v **XSPF** formátu, které si pak lidé naimportuji, nebo překompilovat program s vlastním nastavením kanálu/stanic, které najdete ve funkci `getDefaultChannels()` v souboru `Channels.cs`. Tyto kanály/stanice se pak použijí v případě, že program při vstupu do nastavení zjistí, že seznam kanálu/stanic je prázdný.

**Q:** Co když nemám **Naplánované úlohy**? (Windows osekán pomoci nLite)<br />
**A:** V tom případě máte smůlu.

**Q:** Jaký je rozdíl mezi **MPEG TS** a **MPEG PS**?<br />
**A:** **MPEG TS (Transport Stream)** se používá pro přenášení **MPEG** záznamu v prostředí, kde mohou vznikat chyby, např.: **DVB-T**, streamování po sítí, atp., protože v sobě obsahuje možnost opravování chyb. Narozdíl od toho se **MPEG PS (Program Stream)** využívá v prostředí, kde opravu chyb dokáže zajistit jiná technologie, např. DVD, video na disku, atp.

**Q:** V čem přehraji soubory s **MPEG TS** a **MPEG PS** kontejnerem?<br />
**A:** **MPEG PS** přehraje jakýkoliv software pro sledování videa. Pro přehrání **MPEG TS** kontejnerů, je již potřebný většinou nějaký plugin. Pro programy používající DirectShow, jako například Windows Media Player, BSPlayer, MV2Player a jiné lze použít Haali Media Splitter. Přehrátí **MPEG TS** bez instalace pluginu umí třeba **VLC**, nebo přeportovaný Linuxový **MPlayer** na Windows.<br />
<br />
Haali Media Splitter najdete na: http://haali.cs.msu.ru/mkv/ (odkaz na stáhnutí je vpavo nahoře).

**Q:** Je program funkční s MS Windows Vista a novějším?<br />
**A:** Teoreticky by mělo **Nahrávadlo** fungovat i na **MS Windows Vista** a novějším OS. Od Visty mají Windows novou správu **Naplánovaných úloh**, ale naštěstí je kompatibilní s tou co je ve **MS Windows XP**. Problém by mohl nastat, pokud se naplánováná úloha upraví v **Naplánovaných úlohách**, ale to nemám vyzkoušené. Jediný známý problém je, že po spuštění nahrávání se informace o nahrávání smažou, tj. není možné již smazat nahrávání. V **MS Windows XP** to funguje tak, že se informace o nahrávání smažou až po skončení nahrávání.

**Q:** Co té vedlo k napsání aplikace?<br />
**A:** Mít možnost si dopředu nastavit nahrávání, páč jsem člověk zapomínavý. :o)

**Q:** Jak je to s šiřitelností a zdrojovými kódy?<br />
**A:** Aplikaci šiřte dle libosti, jen bacha na hesla v `config.xml`, aby jste to nešířili se svým loginem a heslem. Zdrojáky jsou k dispozici pod **Apache 2.0 licencí**. Jinak je to naprogramovaný v `C#` v prostředí **MS Visual Studia .NET 2005 SP1** na **.NET 2.0** frameworku.

## Licence

    Copyright 2007 - 2009 Martin Sloup

    Licensed under the Apache License, Version 2.0 (the "License");
    you may not use this file except in compliance with the License.
    You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

    Unless required by applicable law or agreed to in writing, software
    distributed under the License is distributed on an "AS IS" BASIS,
    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
    See the License for the specific language governing permissions and
    limitations under the License.
