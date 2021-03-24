% Author: Loizos Michael
% Date: 2017-06-10

selector(_,_,_).

selection(KB,Xs,InOut,Fs) :-
   InOut=((InR,OutR),(InT,OutT),(InE,OutE)),
   findall(X, ( member(X,Xs),
      elaborate(KB,X,Elab),
      Elab=(ElabR,ElabT,ElabE),
      subset(InR,ElabR),
      subtract(OutR,ElabR,OutR),
      subset(InT,ElabT),
      subtract(OutT,ElabT,OutT),
      forall(member((T,InTEs),InE), (
         member(why(T,TEs),ElabE),
         subset(InTEs,TEs) )),
      forall(member((T,OutTEs),OutE), (
         member(why(T,TEs),ElabE),
         subtract(OutTEs,TEs,OutTEs) ))
   ), Fs).