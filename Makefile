name := Libraries.Operation.dll 
thisdir := .
cmd_library := -t:library
cmd_out := -out:$(name)
cmd_compiler := dmcs
sources := *.cs 

build: $(sources)
	dmcs -optimize $(cmd_library) $(cmd_out) $(sources)
debug: $(sources)
	dmcs -debug $(cmd_library) $(cmd_out) $(sources)
.PHONY: clean
clean:
	-rm -f *.dll *.mdb
