#!/usr/bin/env python

import sys
from pandocfilters import toJSONFilter, walk, Header, Para, RawBlock, RawInline

lastheader = []

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
    if key == 'BlockQuote':
        header = [RawInline('latex', r'\begin{block}{')]
        def extract_title(key, value, format, meta):
            if key == 'Header':
                header.extend(value[2])
                return []
            return process(key, value, format, meta)
        content = walk(value, extract_title, format, meta)
        header.append(RawInline('latex', '}'))
        header = [Para(header)]
        footer = [RawBlock('latex', r'\end{block}')]
        return header + content + footer
    if key == 'Header' and value[0] == 1:
        global lastheader
        lastheader = Header(*value)
    if key == 'HorizontalRule':
        return walk(lastheader, process, format, meta)

if __name__ == '__main__':
    toJSONFilter(process)
