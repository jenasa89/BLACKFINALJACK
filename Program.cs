using System;
using System.Collections.Generic;
using System.Linq;

class Card
{
    public string Suit { get; set; } = string.Empty; // Evita valores nulos
    public string Rank { get; set; } = string.Empty; // Evita valores nulos
    public int Value
    {
        get
        {
            if (int.TryParse(Rank, out int number)) return number;
            if (Rank == "A") return 11;
            return 10;
        }
    }
    public override string ToString() => $"{Rank} de {Suit}";
}

class Deck
{
    private List<Card> cards;
    private static readonly string[] suits = { "Corazones", "Diamantes", "Tréboles", "Picas" };
    private static readonly string[] ranks = { "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K", "A" };
    
    public Deck()
    {
        cards = new List<Card>();
        foreach (var suit in suits)
            foreach (var rank in ranks)
                cards.Add(new Card { Suit = suit, Rank = rank });
        Shuffle();
    }
    
    public void Shuffle()
    {
        Random rng = new Random();
        cards = cards.OrderBy(c => rng.Next()).ToList();
    }
    
    public Card DrawCard()
    {
        if (cards.Count == 0) throw new InvalidOperationException("No quedan cartas en el mazo.");
        var card = cards[0];
        cards.RemoveAt(0);
        return card;
    }
}

class Player
{
    public string Name { get; set; }
    public List<Card> Hand { get; private set; }
    public bool IsDealer { get; set; }
    
    public Player(string name, bool isDealer = false)
    {
        Name = name;
        IsDealer = isDealer;
        Hand = new List<Card>();
    }
    
    public void TakeCard(Card card) => Hand.Add(card);
    
    public int HandValue()
    {
        int total = Hand.Sum(c => c.Value);
        int aces = Hand.Count(c => c.Rank == "A");
        while (total > 21 && aces > 0)
        {
            total -= 10;
            aces--;
        }
        return total;
    }
    
    public override string ToString() => $"{Name}: {string.Join(", ", Hand)} (Valor: {HandValue()})";
}

class BlackjackGame
{
    private Deck deck;
    private Player player;
    private Player dealer;
    
    public BlackjackGame()
    {
        deck = new Deck();
        player = new Player("Jugador");
        dealer = new Player("Crupier", true);
    }
    
    public void Start()
    {
        Console.WriteLine("Bienvenido al Blackjack!");
        player.TakeCard(deck.DrawCard());
        player.TakeCard(deck.DrawCard());
        dealer.TakeCard(deck.DrawCard());
        dealer.TakeCard(deck.DrawCard());
        
        Console.WriteLine(player);
        if (dealer.Hand.Count > 0)
        {
            Console.WriteLine($"Crupier: {dealer.Hand[0]}, [Carta Oculta]");
        }
        else
        {
            Console.WriteLine("El crupier no tiene cartas.");
        }
        
        PlayerTurn();
        DealerTurn();
        DetermineWinner();
    }
    
    private void PlayerTurn()
    {
        while (true)
        {
            Console.Write("¿Quieres pedir carta (p) o quedarte (q)? ");
            string input = Console.ReadLine().ToLower();
            if (input == "p")
            {
                player.TakeCard(deck.DrawCard());
                Console.WriteLine(player);
                if (player.HandValue() > 21)
                {
                    Console.WriteLine("Te pasaste de 21, pierdes.");
                    return;
                }
            }
            else if (input == "q")
                break;
        }
    }
    
    private void DealerTurn()
    {
        Console.WriteLine(dealer);
        while (dealer.HandValue() < 17)
        {
            dealer.TakeCard(deck.DrawCard());
            Console.WriteLine(dealer);
        }
    }
    
    private void DetermineWinner()
    {
        int playerScore = player.HandValue();
        int dealerScore = dealer.HandValue();
        
        if (playerScore > 21)
            Console.WriteLine("Has perdido.");
        else if (dealerScore > 21 || playerScore > dealerScore)
            Console.WriteLine("¡Has ganado!");
        else if (playerScore == dealerScore)
            Console.WriteLine("Empate.");
        else
            Console.WriteLine("Has perdido.");
    }
}

class Program
{
    static void Main()
    {
        Console.WriteLine("Bienvenido al Blackjack!");
        BlackjackGame game = new BlackjackGame();
        game.Start();
    }
}
