#!/usr/bin/env python

from pandocfilters import toJSONFilter, Para, RawBlock

def process(key, value, format, meta):
    if key == 'RawBlock':
        type, content = value
        if type == 'tikz':
            header = r'\begin{tikzpicture}[thick,scale=0.8,transform shape]'
            footer = r'\end{tikzpicture}'
            return RawBlock('latex', header+content+footer)
        if type == 'algorithm':
            header = r'{\scriptsize\begin{algorithmic}[1]'
            footer = r'\end{algorithmic}}'
            return RawBlock('latex', header+content+footer)

if __name__ == '__main__':
    toJSONFilter(process)
