using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class PokemonStatsCalculator : MonoBehaviour
{
    BattleManager battleManager;
    BattleHUD battleHUD;
    PokemonPartyManager pokemonPartyManager;
    PokemonAnimatorManager pokemonAnimatorManager;
    PokemonManager pokemonManager;
    public PokemonBase pokemonBase;

    public List<Move> Moves { get; set; }
    public Move CurrentMove {get; set;}
    public Dictionary<Stat, int> Stats { get; private set; }
    public Dictionary<Stat, int> StatBoosts { get; private set; }
    public Condition Status { get; private set; }
    public int StatusTime { get; set; }
    public Condition VolatileStatus { get; private set; }
    public int VolatileStatusTime { get; set; }

    public Queue<string> StatusChanges { get; private set; } = new Queue<string>();
    public event System.Action OnStatusChanged;

    [SerializeField]
    public bool isWild;

    [SerializeField]
    private int myLevel;
    public int Level { get { return myLevel; } set { myLevel = value; } }

    [Header("CurrentStats")]
    public int maxHP;
    public int currentHP;
    public int currentAttack;
    public int currentDefense;
    public int currentSpAttack;
    public int currentSpDefense;
    public int currentSpeed;

    public bool CheckForLevelUp()
    {
        if (Exp > pokemonBase.GetExpForLevel(Level + 1))
        {
            ++Level;
            return true;
        }
        return false;
    }

    public LearnableMove GetLearnableMoveAtCurrLevel()
    {
        return pokemonBase.LearnableMoves.Where(x => x.Level == Level).FirstOrDefault();
    }

    public void LearnMove(LearnableMove moveToLearn)
    {
        if (Moves.Count > 4)
            return;

        Moves.Add(new Move(moveToLearn.Base));
    }

    public Evolution CheckForEvolution()
    {
        return pokemonBase.Evolutions.FirstOrDefault(e => e.RequiredLevel <= Level);
    }

    public void Evolve(Evolution evolution)
    {
        GameObject newPokemon = Instantiate(evolution.EvolvesInto);
        newPokemon.GetComponent<PokemonStatsCalculator>().Level = Level;
        newPokemon.GetComponent<PokemonStatsCalculator>().CalculateStats();
        newPokemon.GetComponent<PokemonStatsCalculator>().isWild = false;
        newPokemon.tag = "PartyPokemon";
        pokemonPartyManager.partyPokemons.Remove(this.gameObject);
        pokemonPartyManager.partyPokemons.Add((newPokemon.gameObject));
        SetPokemonMovesAndExpAfterEvo(newPokemon.GetComponent<PokemonStatsCalculator>());
        newPokemon.SetActive(false);
    }

    public int Exp { get; set; }
    public int CurrentAttack { get { return GetStat(Stat.Attack); } }
    public int CurrentDefense { get { return GetStat(Stat.Defense); } }
    public int CurrentSpAttack { get { return GetStat(Stat.SpAttack); } }
    public int CurrentSpDefense { get { return GetStat(Stat.SpDefense); } }
    public int CurrentSpeed { get { return GetStat(Stat.Speed); } }

    #region StatFormulas
    public int GetPokemonHP()
    {
        return Mathf.FloorToInt(((2 * pokemonBase.MaxHp) + 31 + (252 / 4)) * Level / 100f) + Level + 10;
    }

    #endregion

    private void SetPokemonLevel()
    {
        if(Level == 0)
        {
            Level = Random.Range(1, 50);
        }
    }

    public void SetPokemonMoves()
    {
        if(Moves == null)//needed to avoid double implementation of moves (solution for overwritten by evo system)
        {
            Moves = new List<Move>();
            foreach (var move in pokemonBase.LearnableMoves)
            {
                if (move.Level <= myLevel)
                {
                    Moves.Add(new Move(move.Base));
                }

                if (Moves.Count >= 4)
                    break;
            }
        }
    }

    public void SetPokemonMovesAndExpAfterEvo(PokemonStatsCalculator other)
    {
        other.Moves = new List<Move>();

        foreach (var move in Moves)
        {
            other.Moves.Add(new Move(move.Base));
        }

        other.Exp = Exp;

    }

    //called whenever a pokemon is INSTANTIATED (can use this to avoid start method)
    private void Awake()
    {
        battleManager = FindObjectOfType<BattleManager>();
        battleHUD = FindObjectOfType<BattleHUD>();
        pokemonAnimatorManager = GetComponentInChildren<PokemonAnimatorManager>();
        pokemonManager = GetComponent<PokemonManager>();
        pokemonPartyManager = FindObjectOfType<PokemonPartyManager>();
        if (Level == 0 && isWild)
        {
            SetPokemonLevel();
        }
        Exp = pokemonBase.GetExpForLevel(Level);
    }

    private void Update()
    {
        currentAttack = CurrentAttack;
        currentDefense = CurrentDefense;
        currentSpAttack = CurrentSpAttack;
        currentSpDefense = CurrentSpDefense;
        currentSpeed = CurrentSpeed;
    }

    //called whenever the game is ran AND whenever the gameobject is FIRST toggled
    private void Start()
    {
        SetPokemonMoves();
        CalculateStats();

        ResetStatBoost();
    }

    public void CalculateStats()
    {
        Stats = new Dictionary<Stat, int>();
        Stats.Add(Stat.Attack, Mathf.FloorToInt((0.01f * (2 * pokemonBase.Attack + 31 + (252 / 4)) * Level) + 5));
        Stats.Add(Stat.Defense, Mathf.FloorToInt((0.01f * (2 * pokemonBase.Defense + 31 + (252 / 4)) * Level) + 5));
        Stats.Add(Stat.SpAttack, Mathf.FloorToInt((0.01f * (2 * pokemonBase.SpAttack + 31 + (252 / 4)) * Level) + 5));
        Stats.Add(Stat.SpDefense, Mathf.FloorToInt((0.01f * (2 * pokemonBase.SpDefense + 31 + (252 / 4)) * Level) + 5));
        Stats.Add(Stat.Speed, Mathf.FloorToInt((0.01f * (2 * pokemonBase.Speed + 31 + (252 / 4)) * Level) + 5));

        maxHP = Mathf.FloorToInt(((2 * pokemonBase.MaxHp) + 31 + (252 / 4)) * Level / 100f) + Level + 10;
        currentHP = maxHP;
    }

    private void ResetStatBoost()
    {
        StatBoosts = new Dictionary<Stat, int>()
        {
            {Stat.Attack, 0},
            {Stat.Defense, 0},
            {Stat.SpAttack, 0},
            {Stat.SpDefense, 0},
            {Stat.Speed, 0},
            {Stat.Accuracy, 0},
            {Stat.Evasion, 0},

        };
    }

    int GetStat(Stat stat)
    {
        int statVal = Stats[stat];

        int boost = StatBoosts[stat];
        var boostValues = new float[] { 1f, 1.5f, 2f, 2.5f, 3f, 3.5f, 4f };

        if (boost >= 0)
            statVal = Mathf.FloorToInt(statVal * boostValues[boost]);
        else
            statVal = Mathf.FloorToInt(statVal / boostValues[-boost]);

        return statVal;
    }

    public void Heal()
    {
        currentHP = maxHP;
        foreach (Move move in Moves)
        {
            move.PP = move.Base.maximumPP;
        }
        CureStatus();
        CureVolatileStatus();
    }

    public void ApplyBoosts(List<StatBoost> statBoosts)
    {
        foreach (var statBoost in statBoosts)
        {
            var stat = statBoost.stat;
            var boost = statBoost.boost;

            StatBoosts[stat] = Mathf.Clamp(StatBoosts[stat] + boost, -6, 6);

            if (boost > 0)
                StatusChanges.Enqueue($"{pokemonBase.Name}'s {stat} rose!");
            else
                StatusChanges.Enqueue($"{pokemonBase.Name}'s {stat} fell!");

            Debug.Log($"{stat} has been modified to {StatBoosts[stat]}");
        }
    }

    public DamageDetails TakeDamage(Move move, PokemonStatsCalculator attacker, Condition weather)
    {
        float critical = 1f;
        if (Random.value * 100f <= 6.25f)
            critical = 2f;

        float stab = attacker.pokemonBase.GetType1 == move.Base.Type || attacker.pokemonBase.GetType2 == move.Base.Type ? 1.5f : 1f;

        float typeEffectiveness = TypeChart.GetEffectiveness(move.Base.Type, this.pokemonBase.GetType1) * TypeChart.GetEffectiveness(move.Base.Type, this.pokemonBase.GetType2);

        float weatherMod = weather?.OnDamageModify?.Invoke(this, attacker, move) ?? 1f;

        var damageDetails = new DamageDetails()
        {
            TypeEffectiveness = typeEffectiveness,
            Critical = critical,
            Fainted = false
        };
        float attack = (move.Base.Category == MoveCategory.Special) ? attacker.CurrentSpAttack : attacker.CurrentAttack;
        float defense = (move.Base.Category == MoveCategory.Special) ? CurrentSpDefense : CurrentDefense;

        float modifiers = Random.Range(0.85f, 1f) * typeEffectiveness * critical * weatherMod * stab;
        float a = (2 * attacker.Level + 10) / 250f;
        float d = a * move.Base.Power * ((float)attack / defense) + 2;
        int damage = Mathf.FloorToInt(d * modifiers);

        UpdateHP(damage);
        return damageDetails;
    }

    public bool OnBeforeTurn()
    {
        bool canPerformMove = true;
        if (Status?.OnBeforeMove != null)
        {
            if (!Status.OnBeforeMove(this))
                canPerformMove = false;
        }

        if (VolatileStatus?.OnBeforeMove != null)
        {
            if (!VolatileStatus.OnBeforeMove(this))
                canPerformMove = false;
        }

        return canPerformMove;
    }

    public void OnAfterTurn()
    {
        Status?.OnAfterTurn?.Invoke(this);
        VolatileStatus?.OnAfterTurn?.Invoke(this);
    }

    public void UpdateHP(int damage)
    {
        currentHP = Mathf.Clamp(currentHP - damage, 0, maxHP);

        if (currentHP <= 0)
            pokemonAnimatorManager.PlayTargetAnimation("Faint");
        else
            pokemonAnimatorManager.PlayTargetAnimation("Hit");

    }

    public void SetStatus(ConditionID conditionID)
    {
        if (Status != null) return;

        Status = ConditionsDB.Conditions[conditionID];
        Status?.OnStart?.Invoke(this);
        StatusChanges.Enqueue($"{pokemonBase.Name} {Status.StartMessage}");
        OnStatusChanged?.Invoke();
    }

    public void CureStatus()
    {
        Status = null;
        OnStatusChanged?.Invoke();
    }

    public void SetVolatileStatus(ConditionID conditionID)
    {
        if (VolatileStatus != null) return;

        VolatileStatus = ConditionsDB.Conditions[conditionID];
        VolatileStatus?.OnStart?.Invoke(this);
        StatusChanges.Enqueue($"{pokemonBase.Name} {VolatileStatus.StartMessage}");
    }

    public void CureVolatileStatus()
    {
        VolatileStatus = null;
    }

    public Move GetRandomMove()
    {
        var movesWithPP = Moves.Where(x => x.PP > 0).ToList();
        int r = Random.Range(0, movesWithPP.Count);
        return movesWithPP[r];
    }

    public Move GetStatusMove()
    {
        var move = Moves.Where(x => x.Base.Category == MoveCategory.Status).ToList();
        int r = Random.Range(0, move.Count);
        return move[r];
    }

    public void OnBattleOver()
    {
        VolatileStatus = null;
        ResetStatBoost();
    }
}

public class DamageDetails
{
    public bool Fainted { get; set; }
    public float Critical { get; set; }
    public float TypeEffectiveness { get; set; }

}
