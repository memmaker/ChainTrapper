# ChainTrapper

A physics based game about traps, written in C# / MonoGame / Box2D

## Keys

| Key        | Action                     |
|------------|----------------------------|
| w/a/s/d    | Movement                   |
| e          | Change Weapon              |
| Space      | Use Weapon                 |
| x          | Activate Remote Detonation |
| MouseLeft  | Shoot                      |

## Spielprinzip

Schafe laufen auf komplizierten Wegen von A nach B.

Es gibt Wölfe die diese Schafe reissen wollen.

Der Spieler ist mit Fallen bewaffnet die er aufstellen kann um die Wölfe davon abzuhalten.

Punktzahl ist die Anzahl Schafe die es in's Ziel schafft.

Es sollte nie möglich sein direkt mit tödlicher Gewalt auf die Wölfe zu schiessen.

Der Spieler startet in Echtzeit in einem Level zusammen mit Schafen und Wölfen.

Die Schafe bleiben zuerst am Startpunkt, bis der Spieler ein Startsignal gibt.

Der Spieler muß die Fallen selbst verteilen in dem er auf dem Spielfeld herumläuft.

Die Wölfe ignorieren den Spieler vorerst.

## Skalierung

64 Pixel = 1 Meter

Zielauflösung: 1920x1080


## Level

10 Level

1-2 Level - Spike Hole
3-4 Level - + Exploding Barrel
5+  Level - + Fire Trap

Man wählt die Fallen vor Beginn eines Levels aus.
Und hat generell weniger Fallen als es Wölfe gibt.

Wölfe pro Level 1-10
20 Schafe im ersten Level. Schafsmenge wird von Level zu Level übernommen.

Punktabzug für Schaden an Schafen.
Bonuspunkte für Stil -> Kettenreaktionen.

## Animationen

Vorerst reicht vermutlich ein Frame für die Schafe, die Wölfe und den Spieler von der Seite.
Das können wir dann flippen um nach rechts und links zu laufen.

Für die Fallen braucht es besondere Animations Frames.

## Animationen Phase 1

 * Aufgespiesste Wölfe & Schafe
 * Wolfs Angriff
 * Blutende Schafe
 * Brennende Wölfe & Schafe
 * Laufender Hirte, Wölfe und Schafe
 * Faßexplosion
 * Feuerfalle wird ausgelöst
 * Feuer

## Grafiken

 * Background Grafikn
 * Bäume & Gebüsche
 * Steine, Felsen & Baumstämme
 * Fallen auf dem Feld
    * Loch mit Spikes
    * Fäßer
    * Feuerfalle

## Mögliche Aktionen

Fallen legen passiert sofort.

 * Loch im Boden !
 * Explodierende Fäßer !
 * Feuerfalle !
   
Die Fallen interagieren miteinander:

Explosionen können Feuer auslöschen und Spielfiguren herumschleudern.

Beim herumschleudern kann man in ein Loch fallen.

Ein Faß das mit Feuer in Kontakt kommt explodiert.

### Extensions

 * Bärenfalle
 * Großer Hammer
 * Riesiger rollender Stein 
 * Explodierendes Schaf
 * (Heilige Handgranate)
 * Pflockfalle
 * Gift

## Level Design

Alles frei platzierbar, wahlweise mit Snap to Grid.

 * Wand Elemente
 * Deko Elemente
 * Fallen in der Umgebung
 * Start & Ziel Punkte
 * Spawn Punkte für die Wölfe
 * Pfad für die Schafe

