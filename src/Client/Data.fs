﻿module Froggy.Dnd5e.Data

type RollSpec =
  | Sum of n:int * die:int
  | SumBestNofM of n:int * m:int * die:int
  | Compound of rolls: RollSpec list * bonus: int

type RollResolver = RollSpec -> int

let rec resolve (r: int -> int) = function
  | RollSpec.Sum(n, die) ->
    seq { for _ in 1..n -> r die } |> Seq.sum
  | RollSpec.SumBestNofM(n, m, die) ->
    seq { for _ in 1..n -> r die } |> Seq.sortDescending |> Seq.take m |> Seq.sum
  | RollSpec.Compound(rolls,  bonus) ->
    rolls |> Seq.sumBy (resolve r) |> (+) bonus

type StatId = Str | Dex | Con | Int | Wis | Cha
type ClassId = Bard | Barbarian | Cleric | Druid | Fighter | Monk | Paladin | Ranger | Rogue | Sorcerer | Warlock | Wizard
type ClassData = { Id: ClassId; StringRep: string; AverageHP: int; Subclasses: string list }
  with
  static member Table =
    [
      // ClassId, string rep, average HP
      Bard, "Bard", 5, ["Lore Bard"; "Valor Bard"]
      Barbarian, "Barbarian", 7, ["Barbearian"; "Ancestor Barbarian"; "Zealot"; "Berserker"; "Battlerager"]
      Cleric, "Cleric", 5, ["Knowledge Cleric"; "Life Cleric"; "Light Cleric"; "Nature Cleric"; "Tempest Cleric"; "Trickery Cleric"; "War Cleric"; "Death Cleric"; "Arcana Cleric"; "Forge Cleric"; "Grave Cleric"]
      Druid, "Druid", 5, ["Moon Druid"; "Shepherd Druid"; "Dream Druid"]
      Fighter, "Fighter", 6, ["Eldritch Knight"; "Battlemaster"; "Champion"; "Cavalier"; "Samurai"; "Purple Dragon Knight"; "Banneret"]
      Monk, "Monk", 5, ["Shadow Monk"; "Long Death Monk"; "Open Hand Monk"; "Elemental Monk"]
      Paladin, "Paladin", 6, ["Paladin of Devotion"; "Paladin of Ancients"; "Paladin of Vengeance"; "Paladin of Conquest"]
      Ranger, "Ranger", 6, ["Hunter"; "Beastmaster"; "Gloomstalker"]
      Rogue, "Rogue", 5, ["Thief"; "Assassin"; "Swashbuckler"; "Mastermind"; "Inquisitive"]
      Sorcerer, "Sorcerer", 4, ["Wild Mage"; "Shadow Sorcerer"; "Divine Soul"; "Dragon Sorcerer"]
      Wizard, "Wizard", 4, ["Abjuror"; "Conjuror"; "Diviner"; "Evoker"; "Enchanter"; "Illusionist"; "Necromancer"; "Transmuter"; "Bladesinger"; "War Mage"]
      Warlock, "Warlock", 5, ["Fiendlock"; "Celestialock"; "Cthulock"; "Feylock"; "Deathlock"; "Hexblade"; "Blade Pact"; "Tome Pact"; "Chain Pact"; "Bladelock"; "Tomelock"; "Chainlock"]
    ]
    |> List.map (fun (id, str, hp, subclasses) -> { Id = id; StringRep = str; AverageHP = hp; Subclasses = subclasses })

type StatArray = {
    Str: int
    Dex: int
    Con: int
    Int: int
    Wis: int
    Cha: int
  }

type StatMod = { Stat: StatId; Bonus: int }
type RaceData = { Name: string; Mods: StatMod list; Trait: string option }

type ClassLevel = { Id: ClassId; Level: int }

type StatBlock = {
    Name : string
    Stats: StatArray
    HP: int
    Race: RaceData option
    XP: int
    Levels: ClassLevel list
    IntendedLevels: ClassLevel list
    Subclasses: (ClassId * string) list
    Notes: string list
  }
let StatBlockEmpty = {
    Name = "Unnamed"
    Stats =
      {
      Str = 10
      Dex = 10
      Con = 10
      Int = 10
      Wis = 10
      Cha = 10
      }
    Race = None
    HP = 1
    XP = 0
    Levels = []
    IntendedLevels = []
    Notes = []
    Subclasses = []
  }

type State = {
    Current: int option
    Party: StatBlock list
  }
let StateEmpty = { Current = None; Party = [] }

type PCXP = { Level: int; XPRequired: int }
  with
  static member Table =
    [
      // level, XP required, daily XP budget, easy, medium, hard, deadly
      0, 0
      1, 0
      2, 300
      3, 900
      4, 2700
      5, 6500
      6, 14000
      7, 23000
      8, 34000
      9, 48000
      10, 64000
      11, 85000
      12, 100000
      13, 120000
      14, 140000
      15, 165000
      16, 195000
      17, 225000
      18, 265000
      19, 305000
      20, 355000
      ] |> List.map (fun (level, xp) -> { Level = level; XPRequired = xp })

let combatBonus statVal =
  (statVal/2) - 5
let skillBonus statVal =
  ((statVal+1)/2) - 5
