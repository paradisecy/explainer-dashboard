:- initialization main.
main :- 
    working_directory(_, 'c:/Users/andreas/Downloads/prolog_files/prolog_files'),
    consult('service.pl'),
    reasonSRV('data/callTestt.kb','data/callTestt.kb',[bird(test)]),
    halt().