# Um�l� inteligence pro deskovou hru Warlight
## Abstrakt
Warlight, inspirovan� deskovou hrou Risk, p�edstavuje
v�zvu pro tvorbu um�l� inteligence z d�vodu obrovsk�ho branching
faktoru.

Pr�ce implementuje bota do t�to hry, kter� je schopn� hr�t vyrovnanou hru
s alespo� m�n� zku�en�m hr��em. Sou��st� je tak� simul�tor, mo�nost
hry proti um�l�mu hr��i i proti jin�mu lidsk�mu hr��i ve form� hotseat (multiplayer hry na jednom po��ta�i).
Pr�ce je vedena tak, aby umo�nila pou�it� tohoto frameworku pro dal�� v�voj a testov�n� bot�.

## �vod
<!---popis kontextu, kter�ho se pr�ce t�k� - popis hry, souvislost s riskem--->

### Z�kladn� informace o h�e
Warlight, inspirovan� deskovou hrou Risk, je hra pro v�ce hr��� odehr�vaj�c� se na
map� rozd�len� na regiony. Ty se shlukuj� do super region� (kontinent�).
C�lem ka�d�ho hr��e je dob�t v�echny regiony vlastn�n� ostatn�mi hr��i.
Na za��tku hry si hr�� vol� regiony, ze kter�ch bude dob�vat dal��.
Ka�d� kolo si hr�� na sv� �zem� stav� jednotky.
Pokud ovl�d� n�jak� super region, m��e si jich postavit v�ce.
Pot� �to�� na sv� sousedy, pop��pad� p�esouv� jednotky. V�po�et ztr�t jednotek p�i �toku na obou stran�ch
je pravd�podobnostn�, kde malou v�hodu maj� br�n�c� jednotky.

### Motivace a obsah pr�ce
<!--- motivace k vytvo�en� pr�ce --->
I p�es existenci sout�e vypsan� Riddles.io na tvorbu AI je tato oblast nep��li� zmapovan�.
D�vodem je nezve�ej�ov�n� implementovan�ch prac� a jejich mal�, �i v�bec ��dn� dokumentace.
Neprob�danost, spolu s velk�m branching faktorem hry, m� motivuje k pokusu o vytvo�en� AI,
kter� bude schopn� hr�t vyrovnanou hru s lidsk�m hr��em.

<!--- co specifi�t�ji je v m� pr�ci, tro�ku jak --->
Pr�ce implementuje AI do t�to hry pou�it�m modifikovan�ho paraleln�ho algoritmu Monte Carlo tree search.
Pro snadn�j�� v�voj je p�id�n simul�tor pro pozorov�n� her bot� proti sob�.
U�ivatel si m��e s�m vyzkou�et hru ve form� singleplayer nebo hotseat multiplayer hry (v�ce hr��� na jednom po��ta�i).

### Struktura pr�ce
- **Popis hry** - c�lem prvn� kapitoly popsat �ten��i hru Warlight
- **Related works** - ???
- **Implementace** - t�et� kapitola popisuje softwarov� komponenty pou�it�
p�i vytvo�en� t�to pr�ce, jejich v�znam a interakci.
- **Um�l� inteligence** - ve �tvrt� kapitole je rozebr�na volba um�l� inteligence a detailn�
zvolen� p��stup k jej� tvorb�.
V t�to sekci tak� rozebereme jej� po��nan� v ur�it�ch hern�ch situac�ch a p��padnou neoptimalitu.
Na konci t�to kapitoly jsou demonstrov�ny v�sledky hry t�to AI proti lidsk�m hr���m i jin�m AI.
- **Z�v�r** - z�v�re�n� kapitola obsahuje zhodnocen� cel�ho projektu. D�le je uvedeno
mo�n� nav�z�n� na tuto pr�ci.

## Obsah pr�ce
1. [�vod]
2. [Popis hry]
3. Related works
4. [Implementace]
5. [Um�l� inteligence]
6. Z�v�r pr�ce
7. [Seznam pou�it� literatury]

## Popis hry
Pravidla hry Warlight jsou relativn� voln�, hra se d� hr�t na spoustu r�zn�ch nastaven�.
V t�to kapitole si vezmeme nastaven�, kter� jsou pou��v�na v t�to pr�ci,
a pop�eme pravidla hry.

### Mapa
Hra se odehr�v� na map�. Ta se d�l� na *regiony*, nejmen�� �zemn� celky t�to hry.
Ka�d� region m� arm�du, bu� hr��e, kter� ho vlastn�, nebo je neobsazen� a seznam sousedn�ch region�.
Regiony se d�le shlukuj� do v�t��ch �zemn�ch celk�, *super region�*.
Narozd�l od hry Risk, mapou m��e b�t libovoln� neorientovan� graf region�.

### Za��tek hry
Na za��tku hry si hr�� zvol� po��te�n� regiony tak, �e od ka�d�ho super regionu vezme pr�v� jeden.
Tato zvolen� �zem� p�edstavuj� v�choz� body, ze kter�ch bude obsazovat dal��. Jakmile hr�� volbu region� potvrd�,
nelze ji ji� zm�nit.

### Pr�b�h hry
- hr��i se st��daj� po taz�ch
- odehraj�-li v�ichni hr��i sv� tahy, dojde k v�po�tu a n�sledn�mu zm�n�n� aktu�ln�ho stavu hry
- hra kon�� ve chv�li, kdy jeden hr�� dobyl �zem� v�ech ostatn�ch hr���

### Tah
Tah se d�l� na 3 f�ze: deploy, attack, commit. V deploy f�zi hr�� stav� arm�du,
v attack f�zi pos�l� �toky a v commit f�zi potvrzuje p�ede�l� akce.

<img src="turn_graph.png" alt="Turn phases graph" />

#### Deploy
V t�to f�zi hr�� stav� arm�du na j�m vlastn�n�ch regionech.
Hr�� m� ur�en� maxim�ln� po�et jednotek, kter� m��e v dan�m tahu postavit.
Od za��tku hry si hr�� m��e stav�t 5 jednotek. Dobude-li n�jak� super region,
zv��� se mu p��sun jednotek o bonus definovan� super regionem.

#### Attack
V t�to f�zi hr�� �to�� jednotkami v�dy ze sv�ho regionu na region sousedn�.
Po �toku v�dy mus� na regionu z�stat alespo� jedna jednotka,
tedy hr�� m��e za�to�it maxim�ln� s po�tem jednotek - 1 na ka�d�m regionu.
Pokud hr�� za�to�� na sv�j region, budeme tuto akci naz�vat p�esunem jednotek.

#### Commit
Z�v�re�n� f�ze, ve kter� hr�� potvrzuje ve�ker� sv� p�edchoz� akce.
Po tomto potvrzen� nen� mo�n� ji� tahy vr�tit a tah je pova�ov�n za uzav�en�.

### Kolo
Kolo je mno�ina tah� v�ech hr���. Jakmile v�ichni hr��i dokon�� sv� tahy,
spust� se v�po�et kola.

Nejprve se akce zlinearizuj�. To vezme ka�dou f�zi zvl᚝, a tyto akce v�dy
projde v po�ad� ABCABC... (p�smena ozna�uj� hr��e, n�hodn� p�i�azen�).

<img src="linearizing.png" alt="Linearizing algorithm" />

Po linearizaci n�sleduje v�po�et kola. Nejprve jsou spu�t�ny v�echny deploy akce, dojde k
p�id�n� jednotek na zvolen� regiony. Pot� jsou spu�t�ny v�echny attack akce.
V�po�et attack akc� se ��d� n�sleduj�c�mi pravidly:

- Pokud hr�� z regionu X poslal jednotky na region Y, kter� v dan� situaci vlastn� jin� hr��,
dojde k v�po�tu ztr�t jednotek. Ka�d� �to��c� jednotka ma 60% �anci na zabit� br�n�c� jednotky,
ka�d� br�n�c� jednotka m� 70% �anci na zabit� �to��c� jednotky.
    - Pokud dojde zabit� v�ech br�n�c�ch jednotek a n�jak� �to��c� p�e�ij�, zbytek t�chto jednotek je p�esunuto
    na dobyt� �zem� a �to��c� hr�� je nov�m vlastn�kem regionu Y.
    - Pokud nejsou zabity v�echny br�n�c� jednotky, pak p�e�iv�� �to��c� se vr�t� zp�tky na X
    - Pokud jsou zabity v�echny jednotky Y i X, na �zem� Y, proto�e minim�ln� po�et jednotek je 1,
    se p�id� jedna jednotka. Vlastn�ci X i Y z�st�vaj� stejn�.
- Pokud hr�� z regionu X poslal jednotky na region Y, kter� v dan� situaci vlastn� tak� on,
dojde k p�esunu t�chto jednotek, �ili nedojde ke ztr�t�m.
- Pokud hr�� z regionu X poslal jednotky na region Y, ale region X mezit�m
dobyl jin� hr��, akce �toku X->Y je zru�ena.
- Pokud hr�� z X na Y poslal *n* jednotek do �toku, ale jin�m �tokem byla tato �to��c� arm�da oslabena o k jednotek,
dojde k posl�n� *n - k* jednotek

## Um�l� inteligence
V t�to kapitole nejprve zanalyzujeme problematiku hry,
ur��me vhodnou metodu p��stupu k implementaci AI. N�sledn�
uk�eme na�i implementaci pou�it�m zvolen�ho algoritmu,
pop�eme naimplementovanou referen�n� AI.
Na z�v�r otestujeme schopnosti AI a zanalyzujeme v�sledky testov�n�.

### Anal�za
<!--- na po�ad� z�le�� --->
Hra Warlight je v�po�etn� velmi n�ro�n�. Tah je unik�tn�
ur�en mno�inou deploy a sekvenc� attack akc�. Li��-li
se po�ad� attack akc�, m��e to m�t velk� vliv na v�sledek kola.

<!--- velk� branching faktor --->
Zp�sob�, jak odehr�t jeden tah, je velmi mnoho. Jednotky
lze nep�ebern� zp�soby distribuovat na vlastn�n� regiony,
a je�t� v�ce zp�soby je lze pos�lat na regiony sousedn�.

<!--- nedeterminismus �toku --->
Dal��m v�zvou je nedeterminismus �toku. N�hoda p�i v�po�tu
�toku m��e znateln� ovlivnit nov� stav po skon�en� kola.

<!--- volba algoritmu --->
Aby algoritmus dob�e po��tal ve velk�m stavov�m prostoru,
m�l by b�t best-first-search. Aby u�ivatel ne�ekal p��li�
dlouho na odpov�� AI, m�l by b�t stanovena maxim�ln� doba v�po�tu.
Algoritmus by tedy m�l b�t schopen v tento �as vydat nejlep�� odpov��,
kterou dosud na�el. Z t�chto d�vod� byl pro implementaci
AI zvolen algoritmus Monte Carlo tree search.

### Monte Carlo tree search AI
V t�to sekci nejprve z�kladn� pop�eme algoritmus MCTS,
n�sledn� uk�eme jeho �pravy pro hru Warlight. Pro
zv��en� v�konu je prozkoum�n a zvolen jeden z p��stup�
k paralelizaci tohoto algoritmu.

#### �vod do MCTS
Monte Carlo tree search je algoritmus, jeho� c�lem je
naj�t nejlep�� tah v dan�m stavu hry. Pro tento ��el
je stav�n v�po�etn� strom. Jeho vrcholy p�edstavuj�
stavy hry, hrany p�edstavuj� akce, kter� do nich vedou.
Na vrcholu je nav�c ulo�en po�et v�her a po�et celkov�ch her,
kter� se dotkly stavu hry v n�m ulo�en�.
V ko�eni je ulo�en stav hry, ze kter�ho se pokou��me nal�zt
nejlep�� tah.

Algoritmus popisuj� 4 f�ze: selekce, expanze, simulace a zp�tn� propagace.
1. *Selekce* - za�ni v ko�eni, v ka�d�m potomkovi v�dy zvol potomka 
podle ur�it� funkce, dokud nedosp�je� do listu
2. *Expanze* - zvolen�mu listu *selekc�* p�idej *n* potomk� a zvol jednoho z nich
3. *Simulace* - ze zvolen�ho potomka za�ni n�hodn� hr�t, dokud jeden z hr��� neprohraje
4. *Zp�tn� propagace* - propaguj informaci o v�h�e/proh�e zp�t a� do ko�ene

#### Modifikace MCTS
Z�kladn� forma MCTS je pro Warlight st�le nepou�iteln�.
V t�to sekci jsou pops�ny modifikace algoritmu tak,
aby efektivn� nach�zel nejlep�� tah v prost�ed� t�to hry.

##### �pravy v�po�etn�ho stromu
Ve h�e Warlight nejprve v�ichni hr��i odehraj� sv� tahy,
a� pot� dojde k v�po�tu kola. Jak by m�l tedy vypadat v�po�etn� strom?

<!--- vlastn�k vrcholu --->
U ka�d�ho vrcholu proto nav�c ur��me jeho vlastn�ka. To bude
hr��, kter� odehr�l tah vedouc� do tohoto vrcholu. U 
ko�ene definujeme jako vlastn�ka hr��e,
z jeho� perspektivy se sna��me naj�t nejlep�� tah.
Od n�sleduj�c�ch �rovn� hloubky stromu se bude vlastnictv�
v�dy st��dat.

<!--- stav mapy v ka�d�m sud�m vrcholu --->
Stav mapy sta�� m�t ulo�en� v ko�eni a ve vrcholech vlastn�n�ch nep��telem,
proto�e jeho tah je posledn�m tahem kola.

##### Ohodnocovac� funkce

##### Gener�tory akc�

#### Paraleln� MCTS

### Agresivn� bot

### Testov�n�

## Implementace
N�pln� t�to kapitoly je sezn�mit �ten��e se soubory pot�ebn�mi pro hru a
z�kladn�mi komponentami a jejich vztahy.

Projekt je implementov�n v jazyce C\# verze 7.2 pro .NET verze 4.5.

### Soubory
V t�to sekci jsou pops�ny soubory vytv��en� nebo p�ilo�en� k projektu a jejich v�znam.

#### Mapy
Soubory map se nach�z� ve slo�ce *Maps*. Pro reprezentaci mapy jsou pot�eba 4 soubory.
Ke h�e je p�ilo�ena mapa sv�ta, s podobn�mi 4 soubory lze v�ak reprezentovat libovolnou mapu.

P�ilo�en� soubory:
- **World.png** - obr�zek mapy sv�ta. Je prezentov�n u�ivateli.
- **WorldTemplate.png** - obr�zek mapy sv�ta, kde ka�d� region m� p�i�azenou unik�tn� barvu.
Ta slou�� p�i rozpozn�v�n� oblasti, na kterou u�ivatel klikl.
- **World.xml** - obsahuje strukturu mapy sv�ta, popisuje super regiony a jejich bonusy,
regiony, jejich sousedy, ke kter�mu super regionu pat��, po��te�n� arm�dy na regionech
- **WorldColorRegionMapping.xml** - p�i�azuje unik�tn� barvu ka�d�mu regionu

�ablony, podle kter�ch se p�� XML na definici struktury dan� mapy a p�i�azov�n� unik�tn� barvy regionu:
- **Map.xsd** - sch�ma validuj�c� XML se strukturou mapy
- **RegionColorMapping.xsd** - sch�ma validuj�c� XML mapov�n� barvy na region

#### Ulo�en� hry a simul�tor
Ulo�en� hry se nach�zej� ve slo�ce *SavedGames*. Tato slo�ka m� dva podadres��e: *Hotseat* a *Singleplayer*.
Ty ur�uj�, pro jak� typ hry dan� ulo�en� hry slou��.
Ulo�en� hry jsou pojmenov�ny *{��slo hry}.sav*, jedn� se o bin�rn� serializovanou *Game* t��du.

Ulo�en� stav v simul�toru se nach�z� ve slo�ce *Simulator*. Ten je ulo�en op�t pod jm�nem *{��slo hry}.sav*.
Nav�c je, pokud bylo v pr�b�hu simulace pu�t�n� logov�n�, obsah logu k dan� h�e ulo�en pod jm�nem *{��slo hry}.log*.
V tom je ve zjednodu�en� form� zaps�no, pro ka�d� kolo, jak AI ohodnotilo jednotliv� �zem�, a jak vyhodnotilo
kvalitu mo�n�ch tah�.

### Architektura
C�lem t�to sekce je proj�t a popsat hlavn� komponenty pr�ce a jejich fungov�n�.

Solution m� slo�ky *Client* a *Tests*. *Client* obsahuje projekty, kter� jsou pot�eba na stran� UI,
*Tests* obsahuje unit testy. Dal�� projekty nejsou zahrnuty do ��dn� slo�ky.

Projekty, vyjma testovac�ch, t�to pr�ce jsou:
*Common, GameObjectsLib, GameHandlersLib, GameAi.Data, GameAi,
FormatConverters, TheAiGames.EngineCommHandler, Communication.CommandHandler,
Client.Entities, WinformsUI*.

V n�sleduj�c�ch sekc�ch pop�eme tyto projekty, jejich fungov�n� a vztahy.

#### Projekt Common
Zde nalezneme pomocn� t��dy, kter� nach�z� vyu�it� nap��� projektem.

**T��dy**

- *Tree* - generick� n-�rn� strom
- *BidirectionalDictionary* - dictionary umo��uj�c� rychl� vyhled�v�n� jak podle kl��e, tak podle hodnoty.
Jedn� se o wrapper nad dv�ma slovn�ky.
- *IRandomInjectable* - t��d�m, kter� implementuj� tento interface,
je mo�no injectnout sv�j vlastn� n�hodn� gener�tor. To slou�� k testov�n� nedeterministick�ch t��d.
- *ObjectPool* - slou�� k poolov�n� libovoln�ch objekt� za ��elem uvoln�n� tlaku na GC.
P�ed ulo�en�m do poolu dojde k vy�i�t�n� tohoto objektu od referenc� na ciz� objekty,
aby se objekt choval podobn� jako nov� naalokovan� t��da a nedoch�zelo tak k memory leak�m.

#### Projekt GameObjectLib
V t�to knihovn� jsou datov� struktury reprezentuj�c� mapu, hr��e, hru a jej� z�znam.

<img src="game_objects_lib.png" alt="Game objects lib schema" />

Na obr�zku v��e je zn�zorn�n� zjednodu�en� sch�ma popisuj�c� objekty a jejich vztahy.

**T��dy**

- *Map* - reprezentuje mapu hry, obsahuje informaci o v�ech regionech a super regionech
- *Game* - p�edstavuje hru, m� mapu a seznam hr���, co tuto hru hraj�. Obsahuje tak�
informaci o v�ech dosud odehran�ch kolech.
- *Player* - reprezentuje hr��e hry.
- *Turn* - tah hr��e. Obsahuje seznam �tok� a deploy akc�, kter� hr�� v dan�m kole odehr�l.
- *Round* - kolo hry. M� seznam tah�.
- *Attack* - kdo na koho �to�il s jak�m po�tem jednotek. Kv�li nedeterministick�mu v�po�tu
obsahuje tak� informaci o nov�m stavu hry po �toku (*PostAttackMapChange*).

#### Projekt GameHandlersLib
V tomto projektu se nach�z� pomocn� t��dy UI. Proto�e tato logika je p�enosn� i na jin� platformy
ne� desktopov�, je tato logika odd�lena od Winforms UI t��d.
T��dy tohoto projektu se d�l� na dv� skupiny: map handlery a game handlery.

##### Map handlery
Map handlery slou�� k vykreslov�n� zm�n na map� do bitmapy. Staraj� se o vykreslov�n�
po�tu jednotek na regiony, p�ekreslov�n� barev region� a zv�razn�n� region�.

**T��dy**

- *ColoringHandler* - p�ekresluje region na po�adovanou barvu. D�raz byl kladen �asov�
efektivn� implementaci.
- *TextDrawingHandler* - vykresluje text na obr�zek mapy. Pou��v� se k vykreslen� po�tu jednotek na region mapy.
Algoritmus nejprve najde region, na kter� se m� ��slo vypsat. Na tomto regionu pak najde unik�tn� zbarven�
pixel ur�uj�c� kam napsat toto ��slo, a ��slo pak vyp�e.
- *HighlightHandler* - dok�e zv�raznit region. Zv�razn�n� prob�h� tak, �e
algoritmus vykresl� ka�d� 3. pixel regionu mapy na p�edem zvolenou highlight barvu. 
- *MapImageTemplateProcessor* - p�edstavuje low-level mapov�n� mezi obr�zkem a hern� objektem regionu.
Umo��uje pro ka�d� pixel naj�t region, nebo ozn�mit, �e na dan�m m�st� se region nenach�z�.
- *MapImageProcessor* - wrapper nad ostatn�mi handlery, kter� zprost�edkov�v� vol�n�.

##### Game handlery
Game handlery slou�� k obsluze jednotliv�ch ��st� hry.

**T��dy**

- *RoundHandler* - zaji��uje v�po�et zm�n p�i p�echodu
z jednoho kola do druh�ho
- *GameFlowHandler* - wrapper, kter� obsahuje pomocn� metody,
kter� se m��ou z u�ivatelsk�ho rozhran� volat.
- *ActionEnumerator* - aby �la odsimulovan� hra
p�ehr�vat, je pot�eba zajistit p�ehr�v�n� akc� ob�ma sm�ry.
Tato komponenta umo��uje iterovat p�es akce v�ech odehran�ch
kol ob�ma sm�ry.
- *GameRecordHandler* - vyu�it�m ActionEnumeratoru umo��uje
iterovat p�es odehran� hern� kola nebo odehran� hern� akce.
- *BotEvaluationHandler* - spou�t� a zastavuje v�po�et bota p�i simulaci.
- *SimulationFlowHandler* - pomocn� t��da pro UI simul�toru,
zaji��uje provol�v�n� metod do *BotEvaluationHandler*u,
*GameRecordHandleru*

#### Projekt GameAi.Data
Tento projekt obsahuje datov� struktury slou��c� k v�po�tu bota a reprezentaci jeho tahu.
Projekt se d�l� do n�kolika adres���.

Adres�� *EvaluationStructures* obsahuje
struktury, kter� jsou pot�eba pro v�po�et. Struktury *MapMin*, *RegionMin* a *SuperRegionMin* jsou
minifikovan�m ekvivalentem obdobn� pojmenovan�m t��d�m v knihovn� *GameObjectsLib*. P�i jejich
implementaci byl kladen d�raz na minim�ln� velikost. To hraje roli p�i �ast�m kop�rov�n�.
K zaji�t�n� lep��ho v�konu byly m�sto t��d pou�ity struktury. T��da *PlayerPerspective* odpov�d�
pohledu na stav hry z perspektivy jednoho ur�it�ho hr��e.

Adres�� *GameRecording* obsahuje t��dy, kter� jsou zmen�en�mi ekvivalenty t��d z *GameObjectsLib*
slou��c� pro z�znam. Tyto t��dy pak pou��v� AI mimo jin� na n�vrat nejlep��ho nalezen�ho tahu.

#### Projekt GameAi
V tomto projektu jsou komponenty souvisej�c� s implementac�
AI.

D�l� se na n�kolik skupin:
- *ActionGenerators* - gener�tory akc�, neboli sekvenc� deploy jednotek a �tok�,
pro dan� stav hry z perspektivy ur�it�ho hr��e. Tyto gener�tory implementuj� interface
*IActionsGenerator*. Rozli�uj� se podle f�ze hry:
    - *IGameBeginningActionsGenerator* - implementace generuj� akce pro za��tek
    hry
    - *IGameActionsGenerator* - implementace generuj� akce pro ostatn� f�ze hry ne� po��te�n�
- *StructureEvaluators* - jedn� se o implementace ohodnocovac�ch
funkc� pro struktury hry, tedy *RegionMin*, *SuperRegionMin* a *PlayerPerspective*

## Seznam pou�it� literatury
[1] *Parallel Monte-Carlo Tree Search*, Guillaume M.J-B. Chaslot, Mark H.M. Winands, and H. Jaap van den Herik

