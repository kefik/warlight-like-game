# Um�l� inteligence pro deskovou hru Warlight
## Abstrakt
Warlight, inspirovan� deskovou hrou Risk, p�edstavuje
v�zvu pro tvorbu um�l� inteligence z d�vodu obrovsk�ho branching
faktoru.

Pr�ce implementuje um�lou inteligenci do t�to hry schopnou hr�t vyrovnanou hru
s alespo� m�n� zku�en�m hr��em. Sou��st� je tak� simul�tor, mo�nost
hry proti AI i proti jin�mu lidsk�mu hr��i ve form� hotseat (multiplayer hry na jednom po��ta�i).
Pr�ce je navr�ena tak, aby umo�nila pou�it� tohoto frameworku pro dal�� v�voj a testov�n� um�l� inteligence.

## �vod
<!---popis kontextu, kter�ho se pr�ce t�k� - popis hry, souvislost s riskem--->

### Z�kladn� informace o h�e
Warlight, inspirovan� deskovou hrou Risk, je hra pro v�ce hr��� zjednodu�en�
simuluj�c� skute�n� v�le�n� konflikt. Ka�d� hr�� za��n� na 2 �zem�. C�lem
hry je dob�t v�echna �zem� vlastn�n� ostatn�mi hr��i. Ned�lnou sou��st� t�to hry 
je tak� n�hoda, kter� rozhoduje o mno�stv� ztr�t p�i ka�d�m boji.

### Motivace
<!--- motivace k vytvo�en� pr�ce --->
I p�es existenci sout�e vypsan� Riddles.io je oblast tvorby um�l� inteligence pro hru Warlight nep��li� zmapovan�.
D�vodem je nezve�ej�ov�n� existuj�c�ch implementac�, nebo jejich mal� �i v�bec ��dn� dokumentace.
Neprob�danost, spolu s velk�m branching faktorem hry, n�s motivuje k pokusu o vytvo�en� um�l� inteligence,
kter� bude schopn� hr�t vyrovnanou hru alespo� s m�n� zku�en�m hr��em.

### C�l pr�ce
<!--- co specifi�t�ji je v m� pr�ci, tro�ku jak --->
C�lem pr�ce je naimplementovat um�lou inteligenci do hry Warlight.
Pro snadn�j�� v�voj um�l� inteligence je d�l��m c�lem pr�ce p�idat simul�tor
pro pozorov�n� her bot� proti sob� a hru ve form� singleplayer nebo hotseat
multiplayer hry (v�ce hr��� na jednom po��ta�i).

### Related works
Pr�ce [GG Warlight Strategy Guide.pdf] a [LearningWarlightStrategy.pdf] popisuj�,
jak�mi pravidly by se m�l lidsk� hr�� ��dit p�i hran� Warlightu. Poznatky z t�chto prac�
jsou vyu�ity p�i tvorb� gener�toru akc� pro AI.

Pr�ce [MHuntersWarLightStrategyGuide.pdf] obsahuje sadu rad, jak by se um�l� inteligence m�la
chovat v r�zn�ch hern�ch situac�ch. Ty jsou vyu�ity p�i implementaci gener�toru akc� a funkc� ohodnocuj�c�ch stav
v AI.

Pr�ce [Parallelmcts.pdf] rozeb�r� p��stupy k implementaci algoritmu Monte Carlo tree search
paraleln�. Jejich efektivita je m��ena na h�e Go. Prac� zm�n�n� Ko�enov� paralelizace je pou�ita
pro paralelizov�n� v�po�tu v na�� implementaci um�l� inteligence.

### Struktura pr�ce
Pr�ce se skl�d� ze 4 kapitol vyjma �vodu:
- **Pravidla hry** - c�lem prvn� kapitoly je detailn� popsat �ten��i pravidla hry
Warlight.
- **Um�l� inteligence** - ve druh� kapitole zanalyzujeme probl�m tvorby um�l� inteligence do hry Warlight a vybereme vhodn� algoritmus. Zam���me se na obecn� pou�it� algoritmus a jeho modifikace p�izp�soben� znalostem t�to hry.
Nakonec rozebereme v�sledky hry na�� um�l� inteligence proti lidsk�m hr���m i jin�m AI. Zam���me
se na pr�zkum jejich nedostatk� a probl�mov� m�sta.
- **Implementace** - t�et� kapitola popisuje soubory souvisej�c� s prac� a strukturu a v�znam hlavn�ch
implementovan�ch t��d.
- **Z�v�r** - z�v�re�n� kapitola zhodnocuje cel� d�lo. Nakonec uv�d� mo�n� nav�z�n� na tuto pr�ci.

## Obsah pr�ce
1. [Pravidla hry]
2. [Um�l� inteligence]
3. [Implementace]
4. Z�v�r pr�ce
5. [Seznam pou�it� literatury]

## Pravidla hry
Pravidla hry Warlight jsou relativn� voln�, hra se d� hr�t na spoustu r�zn�ch nastaven�, kter� 
se li�� p�edev��m v m��e n�hody p�i �to�en� a zp�sobu volen� region� na za��tku hry.
C�lem t�to kapitoly je popsat pravidla hry Warlight s nastaven�m implementovan�m v t�to pr�ci.

### Mapa
Hra se odehr�v� na map�. Ta se d�l� na *regiony*, nejmen�� �zemn� celky t�to hry.
Ka�d� region m� arm�du, seznam sousedn�ch region�, a bu� hr��e, kter� ho vlastn�, nebo je neobsazen�.
Regiony se d�le shlukuj� do v�t��ch �zemn�ch celk�, *super region�*.
Mapou m��e b�t libovoln� neorientovan� graf region�.

V pr�ci je naimplementov�na jedin� mapa slou��c� pro ��ely testov�n� a hran� - mapa sv�ta.
<img src="World.png" alt="Mapa hry" />

Regiony t�to mapy jsou ohrani�en� �zem� na obr�zku, Super regiony jsou kontinenty - Afrika, Asie, Austr�lie, Evropa, Severn� a Ji�n� Amerika.

### Za��tek hry
Na za��tku hry je pro ka�d�ho hr��e vygenerov�na mno�ina region� tak, �e od ka�d�ho super regionu jsou zvoleny pr�v� 2.
Hr�� si z t�to mno�iny vyb�r� 2 regiony. Tato �zem� p�edstavuj� v�choz� body, ze kter�ch bude obsazovat dal��.
Commitem potvrzuje sv� p�edchoz� akce.

### Pr�b�h hry
Hra se d�l� na hern� kola. Ka�d� z nich
se skl�d� z tah�, kde ka�d� hr�� p�isp�v� do kola pr�v� jedn�m tahem.

B�hem hry se hr��i se st��daj� po taz�ch. Odehraj�-li v�ichni hr��i sv� tahy,
dojde k ukon�en� kola. Pot� dojde k v�po�tu nov�ho stavu hry, tedy v�po�tu ztr�t jednotek a 
p��padn�ch zm�n vlastn�k� region� a k zapo�et� nov�ho kola. Tyto kroky se opakuj�,
dokud jeden hr�� neobsad� regiony v�ech ostatn�ch hr���, a nevyhraje tak hru.

### Tah
Tah se d�l� na 3 f�ze: deploy, attack a commit.
V deploy f�zi hr�� stav� arm�du, v attack f�zi pos�l� �toky a v commit f�zi potvrzuje sv� p�edchoz� akce.

P�echody mezi t�mito f�zemi se ��d� n�sleduj�c�m sch�matem:

<img src="turn_graph.svg" alt="Turn phases graph" />

Po odehr�v�n� deploy f�ze hr�� p�ech�z� do attack f�ze, po jej�m� odehr�n� p�ech�z� do commit f�ze.
Sv�j tah pot� hr�� dokon�uje potvrzen�m sv�ch p�edchoz�ch akc� v comit f�zi.

V deploy, attack i commit f�zi m� hr�� mo�nost sv� zm�ny vr�tit. Mezi f�zemi lze tak proch�zet
i opa�n�m sm�rem. P�ejde-li v�ak hr�� z nap��klad attack f�ze do deploy f�ze, zru�� t�m ve�ker� attack akce,
kter� v t�to f�zi zadal. Podobn� hr�� m��e p�ej�t z commit f�ze do deploy f�ze, nebo v deploy f�zi zru�it
sv� ve�ker� zadan� deploy akce.

#### Deploy f�ze
V t�to f�zi hr�� stav� arm�du na n�m vlastn�n� regiony.

*Deploy akc�* nazveme jev, kdy hr�� postav� nenulov� po�et jednotek na dan� region.
Deploy f�ze se skl�d� z 0 nebo v�ce deploy akc�. Pokud v jedn� deploy f�zi je v�ce deploy akc� stav�j�c� jednotky na stejn� region,
tyto akce jsou slou�eny - v�ce deploy akc� se nahrad�
jednou, kter� postav� sou�et v�ech jednotek postaven�ch deploy akcemi na dan� region.

Hr�� m� ur�en� maxim�ln� po�et jednotek, kter� m��e v dan�m tahu postavit.
Na za��tku hry m��e stav�t 5 jednotek. Dobude-li n�jak� super region,
zv��� se mu p��sun jednotek o bonus definovan� super regionem. Pokud o super region p�ijde,
p�ijde tak� o bonus j�m poskytovan�.

Super regiony mapy sv�ta maj� n�sleduj�c� bonusy:
- Asie - 7
- Evropa - 5
- Severn� Amerika - 5
- Ji�n� Amerika - 2
- Afrika - 3
- Austr�lie - 2

#### Attack f�ze
V t�to f�zi hr�� �to�� arm�dou v�dy ze sv�ho regionu na region sousedn�,
pop��pad� jednotky p�esouv� mezi sv�mi sousedn�mi regiony.

*Attack akc�* nazveme jev, kdy hr�� po�le nenulov� po�et jednotek z j�m vlastn�n�ho regionu
na region sousedn�. Attack f�ze se skl�d� z 0 nebo v�ce attack akc�. Pokud v jedn� attack f�zi je v�ce attack akc� takov�ch,
�e �tok vych�z� ze stejn�ch region� a m��� do stejn�ch region�, pak jsou tyto akce slou�eny - z v�ce se vytvo�� jedna
attack akce, kter� bude ze stejn�ch region� a m��it do stejn�ch region� a arm�da bude sou�et v�ech vyslan�ch jednotek pro dyn� dva regiony.

P�i �toku nelze �to�it s celou arm�dou. Na regionu z�stat alespo� jedna jednotka.

#### Commit f�ze
V t�to f�zi hr�� potvrzuje akce, kter� provedl v deploy a attack f�zi.
Po tomto potvrzen� ji� nen� mo�n� je vr�tit a hr���v tah je pova�ov�n za uzav�en�.

### Kolo
Ka�d� hr�� p�isp�v� do kola pr�v� jedn�m tahem. Jakmile v�ichni ukon�� sv� tahy commitem, spust� se v�po�et kola, kter� aktualizuje hern�
stav.

Tahy posledn�ho kola se nejprve zlinearizuj�, pot� n�sleduje v�po�et zm�n.

#### Linearizace
Linearizace je algoritmus, kter� zjednodu�� vno�enou strukturu tah�. Z kola
vytvo�� linearizovan� kolo. To m� stejnou strukturu jako tah a
obsahuje v�echny deploy a attack akce tah� dan�ho kola, ale ve zm�nen�m po�ad�.

<img src="linearizing.svg" alt="Linearizing algorithm" />

Algoritmus nejprve zlinearizuje deploy akce tak, �e pro ka�d� index *i = 1, ..., maximum(po�et deploy akc� libovoln�ho tahu)* vezme i-t� deploy
akce v�ech tah� dan�ho kola (v libovoln�m po�ad�) a p�id� je do v�stupn�ho seznamu.
Pot� zlinearizuje attack akce tak, �e pro ka�d� index *i = 1, ..., maximum(po�et attack akc� libovoln�ho tahu)* vezme i-t� attack akce
ka�d�ho tahu, a v n�hodn�m po�ad� je p�id� do v�stupn�ho seznamu.
Linearizovan� kolo je tvo�eno dv�ma v��e popsan�mi v�stupn�mi seznamy.

**Pseudok�d**
```
Linearizuj() : kolo
    tahy = { v�echny t | t je odehran� tah }
    zp�eh�zejN�hodn�(tahy);

    // zlinearizuj deploy akce
    deploy = {}
    pro ka�d� index i = 1, ..., maximum(po�et deploy akc� libovoln�ho tahu)
        iDeploy := { i-t� deploy akce v�ech tah� }
        deploy.p�idej(iDeploy)

    // zlinearizuj attack akce
    attack = {}
    pro ka�d� index i = 1, ..., maximum(po�et attack akc� libovoln�ho tahu)
        iAttack := { i-t� attack akce v�ech tah� }
        zp�eh�zejN�hodn�(iAttack)
        deploy.p�idej(iAttack)

    linearizovan�Kolo = (deploy, attack)
    vra� linearizovan�Kolo

```

#### V�po�et zm�n kola
P�i v�po�tu zm�n kola dojde k aktualizaci hern�ho stavu proveden�m v�ech zlinearizovan�ch akc�.

Nejprve jsou spu�t�ny v�echny deploy akce - jednotky jsou p�id�ny na hr��i regiony.

Pot� jsou spu�t�ny v�echny attack akce - dojde k v�po�tu ztr�t jednotek v boji a p��padn� zm�n� vlastn�k� region�.
V�po�et ztr�t se ��d� n�sleduj�c�mi pravidly:
- Ka�d� �to��c� jednotka m� 60% �anci na zabit� br�n�c� jednotky.
- Ka�d� br�n�c� jednotka m� 70% �anci na zabit� �to��c� jednotky.

N�sleduj�c� algoritmus spo��t� v�echny zm�ny zp�soben� �toky a zaktualizuje tak
sou�asn� hern� stav.
Algoritmus implementuje n�sleduj�c� pravidla:
- pokud �to��c� region zm�nil vlastn�ka, tento �tok se neprovede (byl by proveden jednotkami hr��e, kter� �tok neposlal)
- nelze za�to�it tak, �e na regionu nez�stane ��dn� jednotka - v�dy mus� z�stat alespo� 1
- �to��-li hr�� na sv�j region, nedoch�z� ke ztr�t�m na jednotk�ch
- pokud p�i �toku dojde k zabit� �to��c�ch i br�n�c�ch jednotek, na br�n�c� region je p�i�azena 1 jednotka (a nem�n� se jeho majitel)
- pokud p�i �toku jsou zabity v�echny br�n�c�, ale n�jak� �to��c� jednotka p�e�ila, nov�m vlastn�kem regionu je �to��c� hr�� a zbytek
�to��c�ch jednotek se p�esune na dobyt� region
- pokud p�i �toku p�e�ily br�n�c� i �to��c� jednotky, zbytek p�e�iv��ch �to��c�ch jednotek se vr�t� na region, ze kter�ho p�i�ly

**Pseudok�d**
```
spo��tejAttacky(linearizovan�Attacky)
    pro ka�d� attack v linearizovan�Attacky
        X := attack.�to��c�Region
        Y := attack.br�n�c�Region
        �to��c�Hr�� := attack.�to��c�Hr��;

        // �to��c� region zm�nil vlastn�ka
        pokud X.vlastn�k != �to��c�Hr��
            p�esko� tento �tok

        // v�dy mus� zb�t alespo� jedna jednotka na regionu
        re�ln��to��c�Arm�da := minimum(attack.�to��c�Arm�da, X.arm�da - 1)
        br�n�c�Arm�da := Y.arm�da

        // hr�� �to�� na sv�j region
        pokud �to��c�Hr�� == Y.vlastn�k
            p�esu� jednotky
        jinak
            // spo��tej zabit� jednotky
            zabit��to��c�Jednotky :=
                spo��tejZabit��to��c�Jednotky(re�ln��to��c�Arm�da, br�n�c�Arm�da)
            zabit�Br�n�c�Jednotky :=
                spo��tejZabit�Br�n�c�Jednotky(br�n�c�Arm�da, re�ln��to��c�Arm�da)
            
            pokud byly zabity v�echny �to��c� i br�n�c� jednotky
                br�n�c�Arm�da := 1
            jinak pokud byly zabity v�echny br�n�c�, ale �to��c� ne
                Y.vlastn�k := �to��c�Hr��
            jinak pokud p�e�ily i br�n�c� i �to��c�
                vra� se s p�e�iv��mi �to��c�mi jednotkami zp�t na X
            jinak pokud p�e�ily br�n�c�
                // nic ned�lej
        
```

## Um�l� inteligence
<!--- popis kapitoly --->
V t�to kapitole nejprve zanalyzujeme problematiku tvorby um�l� inteligence do hry Warlight a
ur��me vhodn� p��stup k implementaci AI. N�sledn�
uk�eme na�i implementaci a pop�eme vytvo�enou referen�n� AI.
Na z�v�r otestujeme schopnosti na�� AI a zanalyzujeme v�sledky testov�n�.

### Anal�za
C�lem t�to sekce je prozkoumat probl�my implementace um�l� inteligence do hry Warlight.

#### V�po�etn� n�ro�nost
Hra Warlight je v�po�etn� velmi n�ro�n�. Tah se skl�d� z deploy a attack akc�.
Na po�ad� deploy akc� jednotliv�ch tah� nez�le��, po�ad� attack akc� v�ak dok�e znateln�
ovlivnit pr�b�h kola. Jednotky lze nep�ebern� zp�soby distribuovat na vlastn�n� regiony,
a je�t� v�ce zp�soby je lze pos�lat na regiony sousedn�.

Vezmeme-li si nap��klad zjednodu�enou hern� situaci:
Hr�� vlastn� 2 nesoused�c� regiony a ka�d� z nich m� 3 sousedy.
Je-li *n* je po�et jednotek na jednom z jeho region�, pak m��e za�to�it n�sleduj�c�m po�tem zp�sob�:

*(i + 3)! / (i! * 3!)*

V��e uveden� vzorec ur��, kolika zp�soby lze rozd�lit *i* jednotek do 2 skupin - na br�n�c� a �to��c�.
Pro *n = 6*, co� odpov�d� situaci, kdy jeden region m� 7 jednotek (z toho 6 lze poslat),
bude v�sledn� hodnota 56.

Vezmeme-li v �vahu, �e hra m� dva hr��e a protivn�k m� stejn� po�et mo�nost�,
jak za�to�it, budeme m�t branching faktor *56 * 56 = 3136*. Je z�ejm�, �e v�echny
tyto mo�nosti nem��eme prozkoumat a bude pot�eba stavov� prostor zmen�it.

#### N�hoda p�i boji
<!--- nedeterminismus �toku --->
Dal�� v�zvou je nedeterminismus �toku. N�hoda p�i v�po�tu
�toku m��e znateln� ovlivnit nov� stav po skon�en� kola. Jak ji�
bylo �e�eno v sekci pravidla, ka�d� �to��c� jednotka m� v boji 60% �anci
na zabit� br�n�c� jednotky a ka�d� br�n�c� jednotka m� 70% �anci na zabit� jednotky �to��c�.

A�koliv je br�n�n� zd�nliv� v�hodn�j��, podle [najdu tu pr�ci a p�id�m referenci] je strategicky v�hodn�j�� p�istupovat
ke h�e agresivn�ji.

#### Volba algoritmu um�l� inteligence
�asov� doba v�po�tu je zna�n� omezen�. Algoritmus by m�l b�t schopen b�hem n�kolika sekund
naj�t nejlep�� odpov��.

<!--- volba algoritmu --->
Pro na�i pr�ci byl zvolen *Monte Carlo tree search*. Tento algoritmus je best-first search, tedy prozkoum�v� nejprve
odpov�di, kter� se zdaj� b�t nejlep��. V�hodou tohoto p��stupu je, �e 
po ur�it�m �ase je schopen vr�tit nejlep�� dosud nalezen� tah. D�ky t�to vlastnosti
tak� dok�e dob�e pracovat v obrovsk�m stavov�m prostoru, pr�v� proto, �e se soust�ed�
nejprve na nejkvalitn�j�� pokra�ov�n�.

### Monte Carlo tree search AI
V t�to sekci nejprve pop�eme obecn� algoritmus Monte Carlo tree search,
n�sledn� uk�eme jeho �pravy pro hru Warlight. Pro
zv��en� v�konu je prozkoum�n a zvolen jeden z p��stup�
k paralelizaci tohoto algoritmu. V �vahu jsou br�ny pouze hry 1v1,
rozbory hry v�ce ne� 2 hr��� tato AI nepodporuje.

**Terminologie**
- *n� hr��* - hr�� z jeho� pohledu se sna��me naj�t nejlep�� tah
- *nep��telsk� hr��* - hr��, kter� nen� n� hr��

#### �vod do Monte Carlo tree search
Monte Carlo tree search (neboli MCTS) je algoritmus, jeho� c�lem je
naj�t nejlep�� tah pro dan� stav hry. Pro tento ��el
je stav�n v�po�etn� strom. Jeho vrcholy p�edstavuj�
stavy hry, hrany p�edstavuj� akce, kter� do nich vedou.
Ve vrcholu je nav�c ulo�en po�et v�her a po�et her stavu hry ve vrcholu.
V ko�eni je ulo�en stav hry, ze kter�ho se pokou��me nal�zt
nejlep�� tah.
Nejlep�� odpov�� v�dy reprezentuje ta hrana, kter� vede do vrcholu s nejvy���m po�tem her.
Tedy nejlep�� odpov�� pro pozici, kterou prozkoum�v�me, je hrana vedouc� z ko�enedo vrcholu s nejvy���m
po�tem her.

Algoritmus popisuj� 4 f�ze, kter� jsou opakov�ny a� do p�eru�en� v�po�tu:
selekce, expanze, simulace a zp�tn� propagace.

<img src="wiki_mcts.png" alt="Mcts phases" />

##### Selekce
�lohou selekce je naj�t list stromu, kter� bude algoritmus d�le rozv�jet.

**Pseudok�d**
```
selekcePotomka() : vrchol
    vrchol := ko�en;
    dokud vrchol != list opakuj
        vrchol := zvolVhodn�hoPotomka(vrchol.potomci);
    vra� vrchol;
```

A�koliv hlavn� my�lenkou algoritmu je rozv�jet tahy, kter� se jev� b�t nejlep��mi,
je pot�eba tak� rozv�jet m�lo prozkouman� tahy. Kdyby tak algoritmus ne�inil,
mohl by vynechat tah, kter� se zprvu jevil jako �patn�,
ve v�sledku by ale byl nejlep��m tahem v dan� pozici.

[Kocsis a Csaba] navrhli *UCT* (Upper Confidence Bound 1 applied to trees) funkci:

*wi / ni + c * sqrt(ln(Ni) / ni)*, kde

- wi - po�et v�her v dan�m vrcholu
- ni - po�et her v dan�m vrcholu
- Ni - celkov� po�et her (= po�et her v ko�eni)
- c - konstanta, teoreticky rovn� *sqrt(2)*, standardn� volen� empiricky

Pokud *ni* je rovno 0, pak hodnota tohoto vrcholu je *infinity*.
Tato situace nast�v� u expandovan�ho vrcholu, ze kter�ho je�t� nebyla vedena simulace.
Hodnota *infinity* tak v p��t� selekci up�ednostn� tento vrchol p�ed jin�mi.

##### Expanze
��elem expanze je p�idat stavy (nebo stav), kter� se budou d�le prozkoum�vat a vybrat jeden z nich,
kter� se bude rozv�jet ve f�zi Simulace.

**Pseudok�d**
```
expanduj(list)
    p�idejPotomky(list);

    // vra� prvn�ho potomka
    vra� list[1];
```

##### Simulace
C�lem simulace je ohodnotit expanz� vybran� vrchol. Simulace odehr�v�
tahy a� do stavu, kdy jedna ze stran vyhr�la.

Existuj� 2 typy simulace: lehk� a t�k�.

Hlavn� my�lenkou *lehk� simulace* (light playout) je, �e odehraji-li n�hodn� dostate�n� po�et her, ze kter�ch v�dy z�sk�m v�sledek v�hra / prohra,
dok�u takov�mto zp�sobem ohodnotit kvalitu tahu, ze kter�ho jsem n�hodn� odehr�n� vedl.
V�hodou je, �e pro aplikaci lehk� simulace nen� pot�eba zn�t ��dn� detaily hry - sta�� pouze v�d�t ve vhodn� okam�ik, 
kter� strana vyhr�la.
Tento p��stup se v�ak nehod� pro hry s velk�m branching faktorem, proto�e na odehr�n�
dostate�n�ho mno�stv� simulac�, kter� by m�ly vypov�daj�c� hodnotu, nen� dostatek �asu.

*T�k� simulace* (heavy playout) se nam�sto n�hodn�ho odehr�v�n� sna�� volit tahy,
kter� se vyplat� prozkoum�vat. D�ky tomu m� v�sledek takov� simulace mnohem v�t�� v�hu. Definovat takov� tah je ale
obt�n�, nal�zt ho je v�po�etn� n�ro�n� a vy�aduje dobrou znalost hry. Tento typ simulace m� [podle n�koho] lep��
v�sledky u hr�ch s vysok�m branching faktorem.

##### Zp�tn� propagace
Zp�tn� propagace propaguje v�sledek simulace od simulovan�ho vrcholu a� do ko�ene.

#### �pravy MCTS
Z�kladn� forma MCTS je pro Warlight st�le nepou�iteln�.
V t�to sekci jsou pops�ny �pravy algoritmu tak,
aby efektivn� nach�zel nejlep�� tah v prost�ed� t�to hry.

##### V�po�etn� strom
Ve h�e Warlight nejprve v�ichni hr��i odehraj� sv� tahy a
a� pot� dojde k v�po�tu kola. Jak by m�l tedy vypadat v�po�etn� strom?

<!--- vlastn�k vrcholu --->
Hrana bude reprezentovat tah hr��e.
U ka�d�ho vrcholu ur��me nav�c jeho vlastn�ka. To bude
hr��, kter� odehr�l tah vedouc� do tohoto vrcholu.
Vlastn�kem ko�ene a jeho d�t� bude n� hr��.
Od n�sleduj�c�ch �rovn� stromu se bude vlastnictv�
v�dy st��dat za��naje od nep��tele.

<!--- stav mapy v ka�d�m sud�m vrcholu --->
Stav mapy sta�� m�t ulo�en� v ko�eni a ve vrcholech vlastn�n�ch nep��telem, proto�e
jeho tah je posledn�m tahem kola.

<!--- expanze v�dy po dvou --->
Ve h�e Warlight v�dy doch�z� k odehr�n� kola a� po odehr�n� tah� v�ech hr���.
Expanze proto nejprve listu zvolen�mu selekc� p�id� vrcholy ur�en� tahy
na�eho hr��e, a t�m rovnou p�id� jako potomky vrcholy ur�en� tahy hr��e nep��telsk�ho.

<img src="changed_tree.svg" alt="Modified evaluation tree" />

Na obr�zku jsou vrcholy na�eho hr��e reprezentov�ny �lutou barvou, vrcholy
nep��telsk�ho hr��e modrou. Stav hry maj� v sob� ulo�en� pouze modr� vrcholy a ko�en.
��sla *m/n* napsan� na vrchol p�edstavuj� *po�et v�her / po�et her* dan�ho vrcholu.
Modr� vrcholy s po�tem her 0 jsou vrcholy, z nich� nebyla dosud vedena simulace.

##### Simulace
Jak bylo uvedeno v anal�ze, obt�nost hry Warlight spo��v� zejm�na ve vysok�m branching faktoru.
Kv�li n�mu je nevhodn� pou��t n�hodnou simulaci. V�hodn�j�� je zvolit simulaci t�kou,
jej� v�sledek bude v�ce odpov�dat skute�n� kvalit� pozice.

Kv�li velk� hloubce v�po�etn�ho stromu v�ak nelze dokonce ani dohr�t simulaci do konce. Takov�
simulace by v lep��m p��pad� st�la mnoho �asu a v hor��m p��pad� v�bec neskon�ila. �e�en�m je odsimulovat
p�edem ur�en� po�et tah� a ohodnotit v�slednou pozici. Toto hodnocen� bude v intervalu [0, 1], kde 1 bude v�hra
na�eho hr��e a 0 v�hra soupe�e.

Pro z�sk�n� p�edstavy o kvalit� pozice je pot�eba b�t schopen tuto pozici ohodnotit.
V n�sleduj�c� sekci podrobn� probereme funkci, kter� tento probl�m �e��.

###### Ohodnocovac� funkce
C�lem ohodnocovac� funkce je z�skat co nejlep�� p�edstavu o kvalit� pozice z pohledu
libovoln�ho hr��e.

Na�e ohodnocovac� funkce nejprve ohodnot� pozici ka�d�ho hr��e tak,
�e se�te hodnotu v�ech jeho region� a jejich arm�d.

**Pseudok�d**
```
ohodno�PoziciHr��e(hr��) : hodnota
    hodnocen�PoziceHr��e := 0;
    pro ka�d� region takov�, �e hr�� ho vlastn�
        hodnocen�PoziceHr��e += ohodno�Region(region) + c * region.Arm�da;

    vra� hodnocen�PoziceHr��e;
        
```
, kde *c* je re�ln� konstanta.

Tuto hodnotu pot� normalizuje do intervalu (0, 1) jednoduchou formul�:

*normalizovan�Ohodnocen�Hr��e1 = hodnotaPoziceHr��e1 / (hodnotaPoziceHr��e1 + hodnotaPoziceHr��e2)*

Pro �plnost zb�v� ji� jen ohodnotit region.

###### Ohodnocen� regionu
Ohodnocen� regionu se mus� li�it pro za��tek hry, kde jsou vyb�r�ny po��te�n� regiony,
a zbyl� ��sti hry. D�vodem k tomu je fakt, �e se tyto dv� ��sti velmi li��.

P�i za��tku hry:

- nen� dobr� br�t v�ce region� bl�zko vedle sebe
- hodnota super regionu p�id�v� na hodnot� regionu

**Pseudok�d ohodnocovac� funkce regionu p�i za��tku hry**
```
ohodno�RegionP�iza��tku(region, m�jHr��) : ��slo
    hodnota := 0;
    hodnota += a * z�skejHodnotuSuperRegionuP�iZa��tkuHry(region.SuperRegion);

    pokud jsem ji� zvolil n�jak� region
        mojeRegiony := m�jHr��.MojeRegiony;
        minim�ln�Vzd�lenostKM�muRegionu := Min(mojeRegiony, region);

        hodnota += b * min(minim�ln�Vzd�lenostKM�muRegionu, maxim�ln�Vzd�lenostDvouRegion�NaMap� / 2);

    vra� hodnota;
```
, kde *a* a *b* jsou re�ln� konstanty.

Po zapo�et� hry:

- hodnota super regionu p�id�v� na hodnot� regionu
- v�hodn�j�� je u super regionu je vlastnit co nejv�ce region�, nejl�pe cel� super region
- region, kter� pat�� n�jak�mu hr��i, m� v�t�� hodnotu - up�ednostn�n� �to�en� na nep��tele a
br�n�n� vlastn�ch region�

**Pseudok�d ohodnocovac� funkce regionu po zapo�et� hry**
```
ohodno�Region(region, m�jHr��) : ��slo
    hodnota := 0;
    // p�ipo�ti hodnotu super regionu
    hodnota += a * z�skejHodnotuSuperRegionu(region.SuperRegion);
    
    // p�idej bonus za po�et region� pro m�ho hr��e
    hodnota := b * z�skejPo�etRegion�SuperRegionu(m�jHr��, region.SuperRegion);

    // region pat�� nep��teli => p�ipo�ti bonus za to, �e pat�� nep��teli
    pokud region.Vlastn�k je soupe�
        hodnota := b * z�skejPo�etRegion�SuperRegionu(nep��tel, region.SuperRegion);

    // pokud pat�� m� nebo soupe�i, zdvojn�sob hodnotu regionu
    pokud region.Vlastn�k jsem j� nebo soupe�
        hodnota *= 2;

    vra� hodnota;
        
```
, kde *a* a *b* jsou re�ln� konstanty.

Pro ohodnocen� regionu je v�ak st�le pot�eba ohodnotit super region.
Ohodnocen� super regionu, obdobn� jako ohodnocen� regionu, se li�� p�i za��tku hry, a pozd�ji.

P�i za��tku hry v�me, �e:

- je v�hodn� br�t super region, kter� m� m�lo soused�c�ch region�,
proto�e po dobyt� se bude l�pe br�nit
- lep�� je super region s v�ce sousedn�mi super regiony, proto�e m��eme
naru�ovat bonusy nep��tel�m nebo rychle dob�vat dal�� super regiony
- je lep��, kdy� super region m� vy��� bonus
- je nev�hodn� br�t super region, kter� se skl�d� z hodn� region�,
proto�e ho je t�k� dob�t a trv� to dlouho

Tyto znalosti jsou poskl�d�ny do vzorce ohodnocovac� funkce pro super region:

```
hodnotaSuperRegionu := a * bonus + b * sousedn�SuperRegiony - c * sousedn�Regiony - d * regionySuperRegionu
```

kde a, b, c, d jsou re�ln� konstanty.

V pozd�j��ch f�z�ch hry se ohodnocovac� funkce li�� pouze v hodnot�ch konstant.

##### Gener�tory akc�
<!--- motivace --->
Jak ji� bylo zm�n�no v anal�ze, po�et mo�n�ch pokra�ov�n� v t�m�� libovoln�m stavu hry je p��li� velk�.
Algoritmus nem� dostatek �asu na proch�zen� v�ech mo�nost�.
Pot�ebujeme zmen�it stavov� prostor.

<!--- popsat, co je gener�tor akc� a co d�l� --->
Kl��em k tomu je *gener�tor akc�*. To je komponenta,
jej�m� ��elem je nal�zt mno�inu smyslupln�ch tah� pro dan�ho hr��e.

<!-- popsat, jak funguje p�esn� --->
N� ak�n� gener�tor nejprve vygeneruje deploy f�ze, a pro ka�dou z nich
pot� vygeneruje attack f�ze. V�sledkem je kart�zsk� sou�in deploy a attack f�z�, ve kter�m
jsou posl�ze odstran�ny duplik�ty.
Algoritmus tak m�sto generov�n� n�hodn�ch permutac� akc� v tahu pou��v� n�kolik potenci�ln� dobr�ch pokra�ov�n�,
kter� vz�jemn� permutuje a znateln� tak zmen�uje stavov� prostor. 

**Pseudok�d**
```
vygenerujTahy(stavHry) : tahy
    tahy := {}
    
    // vygeneruje mo�nosti, jak ud�lat deploy
    deploySekvence := vygenerujDeploy(stavHry)

    pro ka�dou deploy z deploySekvence
        // p�ehrej deploy sekvenci akc�
        aktualizovan�Stav := p�ehrejDeploy(stavHry, deploy)
    
        // vygeneruj mo�nosti jak za�to�it pro dan� deploy
        �toky := vygenerujMo�nosti�toku(aktualizovn�Stav)

        // p�idej v�echny kombinace deploy a �tok� do tah�
        pro ka�d� �tok z �toky
            tahy.p�idej(deploy, �tok)

    odstra�Duplik�ty(tahy)
    vra� tahy
```

Generov�n� deploy akc� vyu��v� 3 p��stupy: �to�n�, obrann� a expanzivn�.

1. *�to�n�* - postav� jednotky na region soused�c�
s neobsazen�m nebo nep��telsk�m regionem,
kter� je nejcenn�j�� (m� nejvy��� hodnotu podle ohodnocovac� funkce).

2. *Obrann�* - postav� jednotky na nejcenn�j�� regiony takov�,
kter�m hroz� dobyt� nep��telem.

3. *Expanzivn�* - postav� jednotky na region soused�c�
s nejcenn�j��m neobsazen�m regionem.

<!--- z�le�� na po�ad� --->
P�i �to�en�, narozd�l od stav�n� jednotek, z�le�� na po�ad�.
Nap��klad m�me-li pozici, kde X je m�j region s arm�dou 8
a Y je s arm�dou tak� 8, je rozd�l, zdali X za�to�� prvn� na
Y nebo naopak, proto�e obr�nce m� v�hodu.

<!--- p�esouv�n� jednotek z vnitrozem� k okraji --->
Jednotky na na�em regionu, kter� m�
za sousedy tak� pouze na�e regiony, jsou nevyu�it�.
Vyplat� se je p�esouvat k m�st�m, kde se budou moci
zapojit do �toku nebo obrany. N�sleduj�c� pseudok�d tuto funkcionalitu
implementuje.

**Pseudok�d**
```
p�esu�Arm�dyZVnitrozem�(stavHry)
    p�esuny := {}
    pro ka�d� m�j region
        pokud v�ichni sousedi regionu jsou moje regiony
            // najdi nejbli��� region, co nen� m�j
            ciz�Region := najdiCiz�Region(region)

            // najdi cestu k n�mu
            cesta := najdiNejkrat��CestuMezi(region, ciz�Region)

            // najdi prvn� region na t�to cest�
            prvn�RegionNaCest� := cesta[1]

            p�esun := po�liJednotky(region, prvn�RegionNaCest�)

            p�esuny.p�idej(p�esun)
```

P�i generov�n� attack akc� pou��v�me 3 p��stupy TODO:
�to�nou, �to�nou s vy�k�n�m, obrannou.

1. *�to�n�* - z ka�d�ho regionu se v�dy pod�v�me na
nep��telsk� sousedn� regiony, a potom na n� za�neme pos�lat
�toky, dokud na�e �to��c� arm�da je siln�j�� ne� arm�da br�n�c�.

2. *�to�n� s vy�k�n�m* - nejprve provedeme p�esun jednotek
z vnitrozem�, pot� �to��me stejn�, jako v p��pad� �to�n� varianty.
To, za�to��-li na n�s nep��tel, n�m zajist� v�hodu br�n�c�ho v boji.

3. *Obrann�* - nejprve p�esuneme jednotky, pot� za�neme pos�lat �toky.
Ty prov�d�me ale tak, �e na region za�to��me jen s arm�dou, kter�
poraz� tu nep��telskou i v p��pad�, �e by na n�j nep��telsk� hr�� postavil v�echny jednotky,
kter� m��e.

D�ky v��e uveden�m algoritm�m se n�m branching faktor zna�n� zmen�il.
Z p�vodn�ho branching faktoru *56 * 56* dojde ke zmen�en� na *9 * 9**,
kde ��slo 9 reprezentuje v�echny kombinace deploy a attack akc�.
Z pravidla je v�tven� mnohem men��, proto�e generov�n� deploy a attack
akc� vygeneruje neunik�tn� tahy, ve kter�ch jsou duplik�ty odstran�ny.

#### Paraleln� MCTS
I p�es znateln� zmen�en� branching faktoru je stavov� prostor hry st�le
velk�. Pro vy��� v�kon by bylo vhodn� paralelizovat v�po�et p�i hled�n� tahu. 
V t�to sekci prozkoum�me mo�nosti paralelizace algoritmu MCTS a 
zvol�me metodu p��stupu k paralelizaci.

Existuj� 4 druhy paralelizace: listov�, ko�enov�, stromov� s glob�ln�m z�mkem
a stromov� s lok�ln�mi z�mky.

<img src="parallel_mcts.png" />

1. *Listov� paralelizace* - z expandovan�ho vrcholu je vedeno, m�sto jedn� simulace,
simulac� n�kolik. Tento postup je velmi jednoduch� na implementaci, av�ak m�
pro n�s jeden d�le�it� probl�m (podle [1]) - rozehrajeme-li nap��klad 8 simulac�, kde 7
skon�� prohrou, pak v�po�etn� �as str�ven� na 8. z nich byl ztracen�, nebo� po 7 prohr�ch
jsme mohli usoudit, �e to nem� cenu d�le zkou�et.

2. *Ko�enov� paralelizace* - m�sto stav�n� jednoho v�po�etn�ho stromu jich stav�me rovnou n�kolik.
Tyto stromy jsou na sob� zcela nez�visl�. Kdy� vypr�� �as a algoritmus m� vr�tit nejlep�� nalezen� tah,
d�ti ko�en� ve v�ech stromech jsou sjednoceny - jsou-li dva vrcholy stejn�, se�te se jejich po�et v�her a po�et her
a slijou se do jednoho, a v t�chto sjednocen�ch vrcholech se vezme vrchol s nejvy���m po�tem n�v�t�v algoritmu.
Tato paralelizace je op�t velmi jednoduch�.

3. *Stromov� paralelizace s glob�ln�m z�mkem* - na strom je um�st�n z�mek, kter� zak�e p��stup
ke stromu v�ce ne� jednomu vl�knu. Simulace v�ak m��ou prob�hat paraleln�, ostatn� f�ze MCTS v�ak ne.

4. *Stromov� paralelizace s lok�ln�mi z�mky* - stromov� paralelizace s lok�ln�mi z�mky umo��uje p��stup 
ke stromu v�ce vl�kn�m. Kdykoliv vl�kno p�istoup� k vrcholu stromu, zamkne si ho, jakmile ho opust�, odemkne ho.
Budou-li pou�ity jako z�mky spinlocky, proto�e zamyk�n� bude pouze na kr�tkou chv�li, tento p��stup by m�l b�t efektivn�.
�astokr�t se v�ak bude st�vat, �e vl�kna budou proch�zet strom zcela stejn�m zp�sobem - pokud vl�kna za�nou v selekci
paraleln� sestupovat od ko�ene a� k listu, je pravd�podobn�, �e budou sestupovat po stejn� cest�. Po odsimulov�n� by
vzestup byl tak� po stejn� cest�. T�m bychom se dostali do podobn� situace, jakou m�
paralelizace listu - vedli bychom v podstat� paraleln� simulaci.

    Abychom se tomuto vyhnuli, z pr�ce [1] bychom vyu�ili koncept
"virtu�ln� prohry" - kdykoliv vl�kno p�istoup� k vrcholu, p�i�ad� mu virtu�ln� prohru, �im� zmen�� jeho hodnocen�.
P��t� vl�kno tak nep�jde po t� stejn� cest�, pokud d�ky virtu�ln� proh�e je hodnocen� vrcholu hor�� ne� hodnocen�
n�kter�ho jin�ho vrcholu na stejn� �rovni.

Tyto metody paralelizace byly v pr�ci [1] otestov�ny na h�e Go, kter�, podobn� jako Warlight, m� velmi velk� stavov� prostor.

<table>
<tr>
   <th colspan="3">Listov� paralelizace</th>
</tr>
<tr>
   <th>Po�et vl�ken</th>
   <th>Po�et v�her</th>
   <th>Po�et her</th>
</tr>
<tr>
   <td>1</td>
   <td>26.7%</td>
   <td>2000</td>
</tr>
<tr>
   <td>2</td>
   <td>26.8%</td>
   <td>2000</td>
</tr>
<tr>
   <td>4</td>
   <td>32.0%</td>
   <td>1000</td>
</tr>
<tr>
   <td>16</td>
   <td>36.5%</td>
   <td>500</td>
</tr>
</table>

<table>
<tr>
   <th colspan="3">Ko�enov� paralelizace</th>
</tr>
<tr>
   <th>Po�et vl�ken</th>
   <th>Po�et v�her</th>
   <th>Po�et her</th>
</tr>
<tr>
   <td>1</td>
   <td>26.7%</td>
   <td>2000</td>
</tr>
<tr>
   <td>2</td>
   <td>38.0%</td>
   <td>2000</td>
</tr>
<tr>
   <td>4</td>
   <td>46.8%</td>
   <td>2000</td>
</tr>
<tr>
   <td>16</td>
   <td>56.5%</td>
   <td>2000</td>
</tr>
</table>

<table>
<tr>
   <th colspan="3">Stromov� paralelizace s glob�ln�m z�mkem</th>
</tr>
<tr>
   <th>Po�et vl�ken</th>
   <th>Po�et v�her</th>
   <th>Po�et her</th>
</tr>
<tr>
   <td>1</td>
   <td>26.7%</td>
   <td>2000</td>
</tr>
<tr>
   <td>2</td>
   <td>31.3%</td>
   <td>2000</td>
</tr>
<tr>
   <td>4</td>
   <td>37.9%</td>
   <td>2000</td>
</tr>
<tr>
   <td>16</td>
   <td>36.5%</td>
   <td>500</td>
</tr>
</table>

<table>
<tr>
   <th colspan="3">Stromov� paralelizace s lok�ln�mi z�mky</th>
</tr>
<tr>
   <th>Po�et vl�ken</th>
   <th>Po�et v�her</th>
   <th>Po�et her</th>
</tr>
<tr>
   <td>1</td>
   <td>26.7%</td>
   <td>2000</td>
</tr>
<tr>
   <td>2</td>
   <td>33.8%</td>
   <td>2000</td>
</tr>
<tr>
   <td>4</td>
   <td>40.2%</td>
   <td>2000</td>
</tr>
<tr>
   <td>16</td>
   <td>49.9%</td>
   <td>2000</td>
</tr>
</table>

Kv�li dobr�m v�sledk�m a jednoduch� implementaci byla pro na�i pr�ci zvolena "Ko�enov� paralelizace".

### Referen�n� um�l� inteligence
Pro ��ely testov�n� na�eho bota jsou naimplementov�ni 2 dal�� referen�n� boti: SmartRandomBot a AggressiveBot.

1. *SmartRandomBot* - tento bot vyu��v� stejn� gener�tor akc� jako MCTS bot. Narozd�l od n�j v�ak nestav� ��dn� strom,
ale n�hodn� vybere z mno�iny tah�, kter� mu gener�tor akc� vr�t�.

2. *AggressiveBot* - jak ji� n�zev napov�d�, tento bot hraje agresivn�.
Na za��tku tahu v�dy postav� arm�du na sv�j region, kter� soused� s region s nejvy��� cenou ([ref na ohodnocovac� funkci]).
Pot� �to�� na v�echny sousedy takov�, �e maj� slab�� arm�du. Po za�to�en� p�esune v�echny sv� arm�dy z vnitrozem� do okrajov�ch
region�.

### V�sledky
<!-- TODO -->

## Implementace
N�pln� t�to kapitoly je sezn�mit �ten��e se soubory pot�ebn�mi pro hru a
z�kladn�mi komponentami a jejich vztahy. D�raz je kladen na pops�n� jednotliv�ch
funk�n�ch celk� a roz�i�itelnost.

### Pou�it� technologie
Pr�ce je implementovan� pou�it�m C# na platform� .NET.

Seznam pou�it�ch technologi�:
- *Windows Forms* - pou�ito pro vytvo�en� GUI spustiteln�ho na Windowsu i Linuxu
- *NUnit* - unit testovac� framework
- *Moq* - slou�� pro vytv��en� mockovac�ch objekt� v testech
- *Protobuf-Net* - zaji��uje efektivn� bin�rn� serializaci a deserializaci
- *Entity Framework* - ORM pro p��stup k datab�zi

### Struktura pr�ce
Solution je rozd�len do n�kolika projekt�:
- *WinformsUI* - grafick� komponenty zobrazovan� u�ivateli
- *Client.Entities* - datab�zov� entity
- *GameHandlers* - pomocn� jednotky pro UI, staraj� se o backendovou logiku
u�ivatelsk�ch p��kaz� a o hern� v�po�ty (nap�. kola)
- *GameObjects* - reprezentuj� hern� objekty (p�. region, super region, mapu, tah, ...)
- *GameAi* - t��dy slou��c� k v�po�tu um�l� inteligence
- *GameAi.Data* - datov� struktury, kter� um�l� inteligence vyu��v�
- *GameAi.Interfaces* - interfacy pro um�lou inteligenci
- *FormatConverters* - obsahuje pomocn� metody pro konverzi z hern�ch objekt� do objekt�, kter� vyu��v� AI, a naopak
- *Common* - obecn� vyu�iteln� struktury a metody nap��� prac� (nap�. obsahuje *ObjectPool*)
- *Communication.CommandHandling* - dll, kter� obsahuje tokeny p�edstavuj�c� p��kazy od enginu
(nap�. "aktualizuj mapu") a t��dy, kter� um� na z�klad� t�chto token� vykon�vat odpov�daj�c� p��kazy
- *Communication.Shared* - obsahuje interfacy pro *Communication.CommandHandling*  (TODO: p�ejmenovat)
- *TheAiGames.EngineCommHandler* - zprost�edkuje komunikaci s TheAiGames enginem - �te
vstup z TheAiGames enginu a p�ekl�d� ho na tokeny definovan� v projektu *Communication.CommandHandling*
D�le projekty, jejich� jm�no kon�� na *{jm�no}.Tests*, jsou testovac� projekty pro projekt se jm�nem *{jm�no}* ({jm�no} je libovoln� jm�no).
Tedy nap��klad *Common.Tests* je testovac� projekt pro *Common*.

Ve slozce solutionu je nav�c slo�ka *Assets*, ve kter� jsou ulo�eny pomocn� soubory.

### Soubory
V t�to sekci jsou pops�ny soubory vytv��en� nebo p�ilo�en� k projektu a jejich v�znam.

#### Datab�ze
Jako datab�ze je v projektu pou�ita *SQLite*. Do n� se ukl�daj� informace o ulo�en�ch
hr�ch, simulac�ch a map�ch. P�edstavuje ji soubor *Utils.db*.

#### Mapy
Soubory map se nach�z� ve slo�ce *Maps*. Pro reprezentaci mapy jsou pot�eba 4 soubory.
Ke h�e je p�ilo�ena mapa sv�ta, s podobn�mi 4 soubory lze v�ak reprezentovat libovolnou mapu. Soubory pro mapu sv�ta jsou:
- **World.png** - obr�zek mapy sv�ta. Je prezentov�n u�ivateli.
- **WorldTemplate.png** - obr�zek mapy sv�ta, kde ka�d� region m� p�i�azenou unik�tn� barvu.
Ta slou�� p�i rozpozn�v�n� oblasti, na kterou u�ivatel klikl.
- **World.xml** - obsahuje strukturu mapy sv�ta, popisuje super regiony a jejich bonusy,
regiony, jejich sousedy, ke kter�mu super regionu pat��, po��te�n� arm�dy na regionech
- **WorldColorRegionMapping.xml** - p�i�azuje unik�tn� barvu ka�d�mu regionu

�ablony, podle kter�ch se p�� XML na definici struktury dan� mapy a p�i�azov�n� unik�tn� barvy regionu:
- **Map.xsd** - sch�ma validuj�c� XML se strukturou mapy
- **RegionColorMapping.xsd** - sch�ma validuj�c� XML mapov�n� barvy na region

##### P�id�n� nov� mapy
A�koliv je ke h�e p�ilo�en� pouze mapa sv�ta, mapou m��e b�t libovoln�
neorientovan� graf region�.

Pro p�id�n� nov� mapy je pot�eba p�idat 4 soubory - 2 grafick� obr�zky mapy a 2 mapovac� soubory.
Je pot�eba p�idat do slo�ky *Maps*:
- obr�zek mapy
- obr�zek mapy, kde ka�d� region m� unik�tn� barvu
- xml definuj�c� super regiony, regiony a jejich sousedy, kter� je
validn� v��i sch�matu *Map.xsd* (lze se inspirovat v p�ilo�en�m souboru *World.xml*)
- xml mapuj�c� barvu na jm�no regionu, kter� spl�uje *RegionColorMapping.xsd*. V�echna
jm�na region� zde pou��vaj�c�ch mus� nav�c b�t definov�ny v xml uveden�m v p�edchoz�m bod�

Nav�c, aby aplikace byla schopna tuto mapu na��st, mus� b�t p�id�n z�znam do datab�ze *Utils.db* do
tabulky *MapInfos*.

#### Ulo�en� hry a simul�tor
Ulo�en� hry se nach�zej� ve slo�ce *SavedGames*. Tato slo�ka m� dva podadres��e: *Hotseat* a *Singleplayer*.
Ty ur�uj�, pro jak� typ hry dan� ulo�en� hry slou��.
Ulo�en� hry jsou pojmenov�ny *{��slo hry}.sav*, jedn� se o bin�rn� serializovanou *Game* t��du.

Ulo�en� stav v simul�toru se nach�z� ve slo�ce *Simulator*. Ten je ulo�en op�t pod jm�nem *{��slo hry}.sav*.
Nav�c je, pokud bylo v pr�b�hu simulace pu�t�n� logov�n�, obsah logu k dan� h�e ulo�en pod jm�nem *{��slo hry}.log*.
V tom je ve zjednodu�en� form� zaps�no, pro ka�d� kolo, jak AI ohodnotilo jednotliv� �zem�, a jak vyhodnotilo
kvalitu mo�n�ch tah�.

### Hern� struktury
V t�to sekci pop�eme struktury, jejich� ��el je popsat hern�
objekty, stav hry a jej� z�znam.

N�sleduj�c� sch�ma implementuje pravidla hry z kapitoly Pravidla hry [ref].

<img src="game_objects.svg" alt="Hern� objekty" />

Hlavn� komponentou je t��da *Game*. Ta reprezentuje sou�asn� stav hry a 
monitoruje tak� jej� z�znam. Tato t��da m� seznam hr���, seznam odehran�ch kol a sou�asn� stav mapy.

Je pot�eba tak� evidovat z�znam hry, aby hra �la znovu p�ehr�vat a analyzovat.
Proto hra m� seznam kol.

<img src="game_objects_rounds.svg" alt="Z�znam hry" />

Ka�d� kolo se skl�d� z mnoha tah�, kde hr�� p�isp�v� do kola pr�v� jedn�m tahem.
Tah m��e b�t bu� po��te�n� (na za��tku hry), nebo standardn� hern�.
Po��te�n� tah eviduje seznam *Seize* akc�, kter� dan� hr�� provedl.
Standardn� hern� tah eviduje seznam deploy a attack akc�, kter� dan� hr�� odehr�l.

### Um�l� inteligence
V t�to sekci rozebereme naimplementovanou um�lou inteligenci
a pop�eme, jak p�idat novou.

Bot je t��da implementuj�c� interface *IBot*. V programu je pou��v�n interface
*IOnlineBot* implementuj�c� *IBot*, kter� nav�c umo��uje p�eru�it v�po�et v libovolnou dobu.

Jednou z hlavn�ch n�pln� pr�ce je implementovat um�lou inteligenci vyu��vaj�c� Monte Carlo tree search algoritmus.
Pro to slou�� t��da *MonteCarloTreeSearchBot* implementuj�c� *IOnlineBot*. Ta pro v�po�et vyu��v�
*MCTSEvaluationHandler*, jej�m� smyslem je postarat se o paraleln� v�po�et MCTS bota pou�it�m Ko�enov� paralelizace [reference].
Ta stav� odd�len� v�po�etn� stromy, kde o v�po�et ka�d�ho se star� *MCTSTreeHandler*.
Ka�d� z t�chto strom� pro v�po�et pot�ebuje gener�tory akc�, komponentu pro ohodnocen� pozice a komponentu pro v�po�et kola.

1. *Gener�tory akc�* - t��dy implementuj�c� *IGameBeginningActionsGenerator*
nebo *IGameActionsGenerator*, podle toho pro kterou f�zi generuje tahy.
2. *Ohodnocov�n� pozice* - pro tento ��el slou�� t��dy implementuj�c� *IPlayerPerspectiveEvaluator*.
3. *V�po�et nov�ho kola* - �e�� *ProbabilityAwareRoundEvaluator*. V�po�et stavu po kole ve h�e Warlight je nedeterministick�,
m��e tedy dopadnout r�zn�mi zp�soby z�vise pouze na n�hod�. Metoda *EvaluateInExpectedAndWorstCase* proto vypo��t�,
jak by vypadalo kolo z pohledu na�eho hr��e nejl�pe a nejh��e a vr�t� tyto nov� stavy.

Jak ji� bylo �e�eno v kapitole Um�l� inteligence, je pot�eba reprezentovat stav hry.

<img src="evaluation_structures.svg" alt="Struktury pro v�po�et" />

*PlayerPerspective* reprezentuje stav hry z pohledu ur�it�ho hr��e. *MapMin* reprezentuje mapu hry,
*SuperRegionMin* super region a *RegionMin* region. V�echny tyto struktury jsou optimalizovan�
tak, aby m�ly minim�ln� velikost z d�vodu �ast�ho kop�rov�n� b�hem v�po�tu.

#### P�id�n� nov� um�l� inteligence
Pro p�id�n� nov�ho bota je pot�eba:
1. Napsat t��du implementuj�c� interface *IOnlineBot*, ve kter� definuje chov�n� bota.
2. P�idat do enumu *GameBotType* novou polo�kou, p�idat nov� case do metody *GameBotCreator.Create* p�i vytv��en� bota

### GUI
TODO

## Seznam pou�it� literatury
[1] *Parallel Monte-Carlo Tree Search*, Guillaume M.J-B. Chaslot, Mark H.M. Winands, and H. Jaap van den Herik

