# Um�l� inteligence pro deskovou hru Warlight
## Abstrakt
Warlight, inspirovan� deskovou hrou Risk, p�edstavuje
v�zvu pro tvorbu um�l� inteligence z d�vodu obrovsk�ho branching
faktoru.

Pr�ce implementuje um�lou inteligenci do t�to hry schopnou hr�t vyrovnanou hru
s alespo� m�n� zku�en�m hr��em. Sou��st� je tak� simul�tor, mo�nost
hry proti UI i proti jin�mu lidsk�mu hr��i ve form� hotseat (multiplayer hry na jednom po��ta�i).
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
Na za��tku hry je pro ka�d�ho hr��e vygenerov�na mno�ina region� tak, �e od ka�d�ho super regionu jsou zvoleny pr�v� 2 regiony.
Hr�� si z t�to mno�iny vyb�r� 2 regiony. Tato �zem� p�edstavuj� v�choz� body, ze kter�ch bude obsazovat dal��.
Commitem potvrzuje sv� p�edchoz� akce

### Pr�b�h hry
Hra se d�l� na hern� kola. Ka�d� hern� kolo
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
- Austr�lie - 3

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

Algoritmus nejprve zlinearizuje deploy akce tak, �e pro ka�d� i vezme i-t� deploy
akce v�ech tah� dan�ho kola (v po�ad�, ve kter�m jsou tahy zaps�ny) a p�id� je do v�stupn�ho seznamu.
Pot� zlinearizuje attack akce tak, �e pro ka�d� i vezme i-t� attack akce
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
- pokud p�i �toku dojde k zabit� �to��c�ch i br�n�c�ch jednotek, na br�n�c� je p�i�azena 1 jednotka (a nem�n� se jeho majitel)
- pokud p�i �toku jsou zabity v�echny br�n�c�, ale n�jak� �to��c� jednotka p�e�ila, nov�m vlastn�kem regionu je �to��c� hr�� a zbytek
�to��c�ch jednotek se p�esune na dobyt� region
- pokud p�i �toku p�e�ily br�n�c� i �to��c� jednotky, zbytek p�e�iv��ch �to��c�ch jednotek se po �toku vr�t� na region, ze kter�ho p�i�ly

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
ur��me vhodnou metodu p��stupu k implementaci AI. N�sledn�
uk�eme na�i implementaci a pop�eme vytvo�enou referen�n� AI.
Na z�v�r otestujeme schopnosti na�� AI a zanalyzujeme v�sledky testov�n�.

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

<!--- po�adavky na algoritmus --->
Doba v�po�tu je omezen�. Algoritmus by m�l b�t schopen po kr�tk�m
�ase vydat dosud nejlep�� nalezenou odpov��.

<!--- volba algoritmu --->
Pro na�i pr�ci byl zvolen best-first search algoritmus Monte Carlo tree search.

### Monte Carlo tree search AI
V t�to sekci nejprve z�kladn� pop�eme obecn� algoritmus MCTS,
n�sledn� uk�eme jeho �pravy pro hru Warlight. Pro
zv��en� v�konu je prozkoum�n a zvolen jeden z p��stup�
k paralelizaci tohoto algoritmu.

**Terminologie**
- *n� hr��* - hr�� z jeho� pohledu se sna��me naj�t nejlep�� tah
- *nep��telsk� hr��* - hr��, kter� nen� n� hr��

#### �vod do MCTS
Monte Carlo tree search je algoritmus, jeho� c�lem je
naj�t nejlep�� tah v dan�m stavu hry. Pro tento ��el
je stav�n v�po�etn� strom. Jeho vrcholy p�edstavuj�
stavy hry, hrany p�edstavuj� akce, kter� do nich vedou.
Na vrcholu je nav�c ulo�en po�et v�her a po�et celkov�ch her,
kter� se dotkly stavu hry v n�m ulo�en�.
V ko�eni je ulo�en stav hry, ze kter�ho se pokou��me nal�zt
nejlep�� tah.
Nejlep�� odpov�� reprezentuje ta hrana, kter� vede do vrcholu s nejvy���m po�tem celkov�ch her.

Algoritmus popisuj� 4 f�ze: selekce, expanze, simulace a zp�tn� propagace.
1. *Selekce* - za�ni v ko�eni, v ka�d�m potomkovi v�dy zvol potomka 
podle p�edem definovan� funkce, dokud nedosp�je� do listu
2. *Expanze* - zvolen�mu listu *selekc�* p�idej *n* d�t� a zvol jedno z nich
3. *Simulace* - ze zvolen�ho potomka za�ni n�hodn� hr�t, dokud jeden z hr��� neprohraje
4. *Zp�tn� propagace* - propaguj informaci o v�h�e/proh�e (0/1) zp�t a� do ko�ene.

<img src="wiki_mcts.png" alt="Mcts phases" />

##### Volba potomka v selekci
Funkce na volbu potomka v selekci pot�ebuje prozkoum�vat nejen nejlep�� varianty,
ale i zkou�et nov� (probl�m *exploitation a exploration*).

[Kocsis a Csaba] navrhli funkci:

*wi / ni + c * sqrt(ln(Ni) / ni)*, kde

- wi - po�et v�her v dan�m vrcholu
- ni - po�et her v dan�m vrcholu
- Ni - celkov� po�et her (= po�et her v ko�eni)
- c - konstanta, teoreticky rovn� *sqrt(2)*

Pokud *ni* je rovno 0, pak hodnota tohoto vrcholu je *infinity*.
Tato situace se stane expandovan�ho vrcholu, ze kter�ho je�t� nebyla vedena simulace.

#### Modifikace MCTS
Z�kladn� forma MCTS je pro Warlight st�le nepou�iteln�.
V t�to sekci jsou pops�ny modifikace algoritmu tak,
aby efektivn� nach�zel nejlep�� tah v prost�ed� t�to hry.

##### �pravy v�po�etn�ho stromu
Ve h�e Warlight nejprve v�ichni hr��i odehraj� sv� tahy,
a� pot� dojde k v�po�tu kola. Jak by m�l tedy vypadat v�po�etn� strom?

<!--- vlastn�k vrcholu --->
Hrana bude reprezentovat tah hr��e.
U ka�d�ho vrcholu ur��me nav�c jeho vlastn�ka. To bude
hr��, kter� odehr�l tah vedouc� do tohoto vrcholu.
Vlastn�kem ko�ene a jeho d�t� bude n� hr��.
Od n�sleduj�c�ch �rovn� hloubky stromu se bude vlastnictv�
v�dy st��dat za��naje od nep��tele.

<!--- stav mapy v ka�d�m sud�m vrcholu --->
Stav mapy sta�� m�t ulo�en� v ko�eni a ve vrcholech vlastn�n�ch nep��telem, proto�e
jeho tah je posledn�m tahem kola.

<img src="changed_tree.svg" alt="Modified evaluation tree" />

Na obr�zku:
- �lut� - hr�� za kter�ho se pokou��me naj�t nejlep�� tah
- modr� - nep��telsk� hr��
- stav hry maj� v sob� ulo�en� pouze modr� vrcholy a ko�en

<!--- expanze v�dy po dvou --->
Ve h�e Warlight v�dy doch�z� k odehr�n� kola a� po odehr�n� tah� v�ech hr���.
Expanze proto nejprve listu zvolen�mu selekc� p�id� vrcholy ur�en� tahy
na�eho hr��e, a t�m rovnou p�id� jako potomky vrcholy ur�en� tahy hr��e nep��telsk�ho.

##### Ohodnocovac� funkce
Vysok� branching faktor a cyklen� znemo��uje pou�it� n�hodn� simulace,
kter� by skon�ila a� ve chv�li v�hry jednoho z hr���.
M�sto toho odsimulujeme p�edem ur�en� po�et tah�, ohodnot�me pozici a zp�tnou propagac� vr�t�me ��slo v intervalu [0, 1]
ur�uj�c� kvalitu pozice.
Tuto hodnotu pak propagujeme do ko�ene.

Abychom mohli z�skat p�ehled o celkov� pozici, pot�ebujeme ohodnotit d�l�� ��sti.

###### Ohodnocen� super regionu
Ohodnocen� super regionu se li�� p�i za��tku hry, a pozd�ji.

P�i za��tku hry, ze znalosti Warlightu, v�me, �e:

- je v�hodn� br�t super region, kter� m� m�lo soused�c�ch region�,
proto�e po dobyt� se bude l�pe br�nit
- lep�� je super region s v�ce sousedn�mi super regiony, proto�e m��eme
naru�ovat bonusy nep��tel�m nebo rychle dob�vat dal�� super regiony
- je lep��, kdy� super region m� bonus
- je nev�hodn� br�t super region, kter� se skl�d� z hodn� region�,
proto�e ho je t�k� dob�t a trv� to dlouho

Tyto znalosti jsou poskl�d�ny do vzorce ohodnocovac� funkce pro super region:

```
hodnota := a * bonus + b * sousedn�_super_regiony - c * sousedn�_regiony - d * regiony_super_regionu
```

kde a, b, c, d jsou re�ln� konstanty.

V pozd�j��ch f�z�ch hry se ohodnocovac� funkce li�� pouze v hodnot�ch konstant.

###### Ohodnocen� regionu
P�i ohodnocov�n� regionu z�le�� tak�, zdali je za��tek hry �i ne.

P�i za��tku hry:

- nen� dobr� br�t v�ce region� bl�zko vedle sebe
- hodnota super regionu m� vliv na hodnotu regionu
<!--- TODO --->

###### Ohodnocen� pozice hr��e
Budeme ohodnocovat ka�d�ho hr��e zvl᚝. Pro ohodnocen� hr��ovy pozice
porovn�me jeho hodnotu s hodnotou druh�ho hr��e.

```
hodnotaHr��e(hr��)
    hodnota := 0

    pro ka�d� region vlastn�n� t�mto hr��em
        hodnota += ohodnocen�Regionu(region)
        hodnota += ohodnocen�Arm�dy(region.arm�da)
    
    vra� hodnota
```

```
ohodnocen�PoziceHr��e1(hr��1, hr��2)
    hodnotaHr��e1 := hodnotaHr��e(hr��1)
    hodnotaHr��e2 := hodnotaHr��e(hr��2)
    
    ohodnocen�Pozice1 := hodnotaHr��e1 / (hodnotaHr��e1 + hodnotaHr��e2)

    vra� ohodnocen�Pozice1
```

##### Gener�tory akc�
<!--- motivace --->
Po�et mo�n�ch pokra�ov�n� v t�m�� libovoln�m stavu hry je p��li� velk�.
Algoritmus nem� dostatek �asu na proch�zen� v�ech mo�nost�.
Pot�ebujeme zmen�it stavov� prostor.

<!--- popsat, co je gener�tor akc� a co d�l� --->
Kl��em k tomu je *gener�tor akc�*. To je softwarov� komponenta,
jej�m� ��elem je nal�zt mno�inu smyslupln�ch tah� pro dan�ho hr��e.

<!-- popsat, jak funguje p�esn� --->
N� ak�n� gener�tor nejprve vygeneruje mo�nosti, jak ud�lat deploy,
potom pro ka�dou z t�chto mo�nost� vygeneruje zp�soby, jka za�to�it.

Algoritmus:
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

###### Generov�n� deploy akc�
Pou��v� 3 p��stupy v generov�n� deploy akc�: �to�n�, obrann� a expanzivn�.

1. *�to�n�* - postav� jednotky na region soused�c�
s neobsazen�m nebo nep��telsk�m regionem,
kter� je nejcenn�j�� (m� nejvy��� hodnotu podle ohodnocovac� funkce).

2. *Obrann�* - postav� jednotky na m� nejcenn�j�� regiony takov�,
kter�m hroz� dobyt� nep��telem.

3. *Expanzivn�* - postav� jednotky na region soused�c�
s nejcenn�j��m neobsazen�m regionem.

###### Generov�n� attack akc�
<!--- z�le�� na po�ad� --->
P�i �to�en�, narozd�l od stav�n� jednotek, z�le�� na po�ad�.
Nap��klad m�me-li pozici, kde X je m�j region s arm�dou 8
a Y je s arm�dou tak� 8, je rozd�l, zdali X z�le�� prvn� na
Y nebo naopak, proto�e obr�nce m� v�hodu.

<!--- p�esouv�n� jednotek z vnitrozem� k okraji --->
Jednotky, kter� jsou na na�em regionu, kter� m�
za sousedy tak� pouze m� regionu, jsou nevyu�it�.
Vyplat� se je p�esouvat k m�st�m, kde se budou moci
zapojit do �toku nebo obrany.

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

P�i generov�n� attack akc� pou��v�me 3 mo�n� varianty:
�to�nou, �to�nou s vy�k�n�m, obrannou.

1. *�to�n�* - z ka�d�ho regionu se v�dy pod�v�me na
nep��telsk� sousedn� regiony, a potom na n� za�neme pos�lat
�toky, dokud na�e �to��c� arm�da je siln�j�� ne� arm�da br�n�c�.

2. *�to�n� s vy�k�n�m* - nejprve provedeme p�esun jednotek
z vnitrozem�, pot� �to��me stejn�, jako v p��pad� �to�n� varianty.

3. *Obrann�* - nejprve p�esuneme jednotky, pot� za�neme pos�lat �toky.
Ty prov�d�me ale tak, �e na region za�to��me jen s arm�dou, kter�
poraz� tu nep��telskou i v p��pad�, �e by na region postavil v�echny jednotky,
kter� m��e.

#### Paraleln� MCTS

### Agresivn� bot a Smart random bot

### V�sledky

## Implementace
N�pln� t�to kapitoly je sezn�mit �ten��e se soubory pot�ebn�mi pro hru a
z�kladn�mi komponentami a jejich vztahy.

Projekt je implementov�n v jazyce C\# verze 7.2 pro .NET verze 4.5.

### Soubory
V t�to sekci jsou pops�ny soubory vytv��en� nebo p�ilo�en� k projektu a jejich v�znam.

#### Datab�ze
Jako datab�ze je v projektu pou�ita *SQLite*. Do n� se ukl�daj� informace o ulo�en�ch
hr�ch, simulac�ch a map�ch. P�edstavuje ji soubor *Utils.db*.

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

#### P�ehled
<img src="assembly_map.svg" alt="Map of assemblys" />

Obr�zek v��e p�edstavuje z�kladn� vztahy mezi projekty.

- *WinformsUI* - grafick� komponenty zobrazovan� u�ivateli
- *Client.Entities* - datab�zov� entity
- *GameHandlers* - pomocn� jednotky pro UI, staraj� se o backendovou logiku
u�ivatelsk�ch p��kaz� a o hern� v�po�ty (nap�. kola)
- *GameObjects* - reprezentuj� hern� objekty (p�. region, super region, mapu, tah, ...)
- *GameAi* - t��dy slou��c� k v�po�tu um�l� inteligence

#### Projekt WinformsUI
Tento projekt obsahuje grafick� komponenty zobrazovan� u�ivateli: formul��e
a souvisej�c� user controly.
Ty jsou ps�ny pou�it�m Windows Forms technologie. D�l� se na dv� logick� celky:
nastavovac� a ingame komponenty.

1. *Nastavovac�* - nach�z� se v adres��i *GameSetup*. Umo��uj� u�ivateli nastavit
hru. Jako pomocn� user controly pou��vaj� komponenty z adres��e *HelperControls*.
Nastavovac� komponenty se staraj� o zalo�en� nov� hry nebo simulace a na�ten� ulo�en� hry
nebo simulace.
2. *InGame* - jsou zobrazov�ny u�ivateli po zalo�en� nebo na�ten� hry nebo simulace.

    O simulaci se star� t��da *SimulatorInGameControl*. Tato t��da je zobrazena 
    u�ivateli po na�ten� nebo vytvo�en� nov� simulace. P�ij�m� u�ivatelovy akce
    a p�epos�l� je d�l komponent� *SimulationFlowHandler* na zpracov�n�.

    O pr�b�h hry se staraj� zb�vaj�c� t��dy. T��da *InGameControl* je zobrazena
    u�ivateli. Ta obsahuje mapu - reprezentov�na t��dou *MapHandlerControl* staraj�c�
    se o u�ivatelskou interakci s mapou, a hern� panel. Hern� panel je reprezentov�n
    ve slo�ce *Phases*. Ty se staraj� o zvl�d�n� jednotliv�ch f�z� tahu.

#### Projekt GameObjectLib
V t�to knihovn� jsou datov� struktury reprezentuj�c� mapu, hr��e, hru a jej� z�znam.

<img src="game_objects_lib.svg" alt="Game objects lib schema" />

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
ne� desktopov�, jsou komponenty odd�leny od Winforms UI t��d.
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
V tomto projektu jsou komponenty souvisej�c� s implementac� AI.

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

