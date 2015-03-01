RST := $(wildcard *.rst)
TEX := $(patsubst %.rst,%.tex,${RST})
PDF := $(patsubst %.rst,%.pdf,${RST})

all : ${PDF}

publish : ${PDF}
	scp $^ sphinx:/afs/matinf.uj.edu.pl/common/tcs/gherman/ko/wyklad/

${PDF} : %.pdf : %.tex
	pdflatex $*
	pdflatex $*

${TEX} : %.tex : %.rst filter.py header.tex
	pandoc $< -t beamer --latex-engine pdflatex --filter filter.py -H header.tex -o $@

.PHONY : all publish
