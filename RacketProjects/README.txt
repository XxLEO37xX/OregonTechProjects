# Lab 3 - Scheme Blackjack

## Author

Leonardo Espinosa

## Course

CST 223 - Concepts of Programming Languages

## Instructor

Professor Helmik

## Description

This project is a simplified Blackjack simulator written in Racket (Scheme).

The program models a two-player Blackjack game where each player follows a strategy that determines whether to hit or stay. The project focuses on higher-order functions, recursion, data abstraction, and strategy testing.

Unlike traditional Blackjack, this simulation:

* Uses card values from 1–10 only
* Does not include aces, splitting, or betting
* Uses an infinite deck with equal probability for each card value
* Treats ties as wins for the house

The goal of the assignment is to compare different Blackjack strategies and observe their behavior through simulation.

---

## Features

### Interactive Gameplay

Allows players to manually decide whether to hit or stay.

```scheme
(blackjack hit? hit?)
```

### Strategy Generation

Implemented `stop-at`, which creates strategies that continue hitting until a specified hand total is reached.

Example:

```scheme
(stop-at 16)
```

This strategy hits while the hand total is less than 16 and stays once the total reaches 16.

### Strategy Testing

Implemented `test-strategy`, which simulates multiple Blackjack games and returns the number of player wins.

Example:

```scheme
(test-strategy (stop-at 16) (stop-at 15) 10)
```

### Strategy Observation

Implemented `watch-player`, which wraps an existing strategy and displays:

* Current hand
* Opponent up-card
* Strategy decision

Example:

```scheme
(watch-player (stop-at 3))
```

This feature is useful for debugging and observing gameplay behavior.

---

## Testing

Unit tests were implemented using RackUnit.

Tests include:

* Hand creation
* Hand totals
* Card addition
* Strategy behavior
* Play-hand execution

When all tests pass, the program displays:

```text
All non-interactive tests passed.
```

---

## Requirements

* Racket
* DrRacket

---

## How to Run the Program

1. Open `lab3.rkt` in DrRacket.
2. Click **Run**.
3. Uncomment the desired function call at the bottom of the source file.
4. Run the file again to execute that example.

Examples:

### Interactive Game

```scheme
(blackjack hit? hit?)
```

### Play Against a Strategy

```scheme
(blackjack hit? (stop-at 16))
```

### Compare Strategies

```scheme
(test-strategy (stop-at 16) (stop-at 15) 10)
```

### Observe Strategy Decisions

```scheme
(test-strategy
 (watch-player (stop-at 3))
 (watch-player (stop-at 3))
 2)
```

---

## Concepts Demonstrated

* Functional Programming
* Higher-Order Functions
* Closures
* Recursion
* Data Abstraction
* Unit Testing with RackUnit
* Strategy Simulation

---

## Files

* `lab3.rkt` - Main source file
* `README.md` - Project documentation
