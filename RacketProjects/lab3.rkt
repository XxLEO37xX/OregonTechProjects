#lang racket
(require rackunit)

; make a new hand
(define (make-new-hand first-card)
  (make-hand first-card first-card))

; deal function
(define (deal)
  (+ 1 (random 10))) 

; make-hand function
(define (make-hand up-card total)
  (cons up-card total))

; hand-up-card function
(define (hand-up-card hand)
  (car hand))

; hand-add-card function
(define (hand-add-card hand new-card)
  (make-hand (hand-up-card hand)
             (+ new-card (hand-total hand))))

; hand-total function
(define (hand-total hand)
  (cdr hand))

(define (stupid-strategy my-hand opponent-up-card)
  (> opponent-up-card 5))

; play hand
(define (play-hand strategy my-hand opponent-up-card)
  (cond ((> (hand-total my-hand) 21) my-hand)
        ((strategy my-hand opponent-up-card)
         (play-hand strategy
                    (hand-add-card my-hand (deal))
                    opponent-up-card))
        (else my-hand)))

;stop at
(define (stop-at n)
  (lambda (my-hand opponent-up-card)
    (< (hand-total my-hand) n)))


; blackjack
(define (blackjack player-strategy house-strategy)
  (let ((house-initial-hand (make-new-hand (deal))))
    (let ((player-hand
           (begin
             (display "Player 1's turn\n")
             (play-hand player-strategy
                        (make-new-hand (deal))
                        (hand-up-card house-initial-hand)))))
      (if (> (hand-total player-hand) 21)
          0
          (let ((house-hand
                 (begin
                   (display "Player 2's turn\n")
                   (play-hand house-strategy
                              house-initial-hand
                              (hand-up-card player-hand)))))
            (cond ((> (hand-total house-hand) 21) 1)
                  ((> (hand-total player-hand)
                      (hand-total house-hand)) 1)
                  (else 0)))))))

;test strategy
(define (test-strategy player-strategy house-strategy n)
  (if (= n 0)
      0
      (+ (blackjack player-strategy house-strategy)
         (test-strategy player-strategy house-strategy (- n 1)))))

;watch player
(define (watch-player strategy)
  (lambda (my-hand opponent-up-card)
    (let ((decision (strategy my-hand opponent-up-card)))
      (display "Up-card: ")
      (display (hand-up-card my-hand))
      (display ", total: ")
      (display (hand-total my-hand))
      (newline)
      (display "Opponent up-card: ")
      (display opponent-up-card)
      (newline)
      (display "Decision: ")
      (display (if decision "hit" "stay"))
      (newline)
      (newline)
      decision)))

; interactive strategy
(define (hit? my-hand opponent-up-card)
  (display "Your up card is ")
  (display (hand-up-card my-hand))
  (newline)
  (display "Your opponent's up card is ")
  (display opponent-up-card)
  (newline)
  (display "Your total is ")
  (display (hand-total my-hand))
  (newline)
  (display "Do you want to hit? (1 for yes / 0 for no) ")
  (define answer (read))
  (and (number? answer) (= answer 1)))

; ---------- TESTS ---------- (gotta make sure code is good to go before the fun starts)

(check-equal? (hand-up-card (make-new-hand 6)) 6)
(check-equal? (hand-total (make-new-hand 6)) 6)

(define h1 (make-new-hand 7))
(define h2 (hand-add-card h1 5))

(check-equal? (hand-up-card h2) 7)
(check-equal? (hand-total h2) 12)

(check-true (stupid-strategy (make-new-hand 10) 8))
(check-false (stupid-strategy (make-new-hand 10) 5))

(check-equal? (hand-total (play-hand (lambda (h o) #f) (make-new-hand 10) 8)) 10)
(check-true (> (hand-total (play-hand (lambda (h o) #t) (make-new-hand 10) 8)) 21))

(display "All non-interactive tests passed.\n")

;code to test problem 2 functionality
;(define s16 (stop-at 16))

;(s16 (make-new-hand 10) 5) ; #t (10 < 16 ==> hit)
;(s16 (make-new-hand 16) 5) ; #f (16 < 16 ==> false ==> stay)
;(s16 (make-new-hand 20) 5) ; #f

;(display "Problem 2 Tests Successful.\n")

;----------------------- RUN PROGRAM -----------------------

;(blackjack hit? hit?)                                                  ;problem 1
;(blackjack hit? (stop-at 16))                                          ;problem 2
;(test-strategy (stop-at 16) (stop-at 15) 10)                           ;problem 3
(test-strategy (watch-player(stop-at 3)) (watch-player(stop-at 3)) 2)  ;problem 4