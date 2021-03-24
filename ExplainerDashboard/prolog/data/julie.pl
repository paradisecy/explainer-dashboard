% Author: Loizos Michael
% Date: 2018-01-16

% ============= Julie, Appointment Assistant

% ============= Library

quantum(60).

quantize(TP,Q,ceiling,QP) :-
    TP=(Y,M,D,H,N),
    N1 is ceiling(N/Q)*Q,
    date_time_stamp(date(Y,M,D,H,N1,0,0,-,-),Stamp),
    QP is rationalize(Stamp/(60*Q)).
quantize(TP,Q,floor,QP) :-
    TP=(Y,M,D,H,N),
    N1 is floor(N/Q)*Q,
    date_time_stamp(date(Y,M,D,H,N1,0,0,-,-),Stamp),
    QP is rationalize(Stamp/(60*Q)).

analogize((QS,QE),Q,(TS,TE)) :-
    !, analogize(QS,Q,TS),
    analogize(QE,Q,TE).
analogize(QP,Q,TP) :-
    Stamp is QP*60*Q,
    stamp_date_time(Stamp,date(Y,M,D,H,N,_,_,_,_),0),
    TP=(Y,M,D,H,N).

within((QS1,QE1),(QS2,QE2)) :-
    !, QS1 >= QS2, QE1 =< QE2.
within(QP1,(QS2,QE2)) :-
    within((QP1,QP1),(QS2,QE2)).

before((_QS1,QE1),(QS2,_QE2)) :-
    !, QE1 =< QS2.
before(QP1,(QS2,QE2)) :-
    before((QP1,QP1),(QS2,QE2)).
before((QS1,QE1),QP2) :-
    before((QS1,QE1),(QP2,QP2)).

overlap(QW1,QW2) :-
    \+before(QW1,QW2), \+before(QW2,QW1).

duration((QS,QE),Q,TD) :-
    TD is (QE-QS)*Q.

tiebreak(S1,S2) :-
    S1 @< S2.

mentions(M1,M2) :-
    sub_string(M1,_,_,_,M2).
