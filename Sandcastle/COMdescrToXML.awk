BEGIN {
    print "<?xml version=\"1.0\"?>"
    print "<doc>"
    print "    <assembly>"
    print "        <name>Selenium</name>"
    print "    </assembly>"
    print "    <members>"
}

{
    if( match($0,/Description\("(.+?)"\)/,d) ) {
        descr = d[1]
        gsub("<","\\&lt;",descr)
        gsub(">","\\&gt;",descr)
        match(FILENAME,/(_[A-Z][A-z]+)\.cs/,f)
        mn = ""
        getline NL
        if( match(NL,/\s([A-Z][A-z]+)\((.*)\);$/,m) ) {
            method = m[1]
            split(m[2],pars,",")
            ps = ""
            for( pi in pars ) {
                par = pars[pi]
#print par
                gsub(/^\s+/,"",par)
                gsub(/\s.+$/,"",par)
                gsub(/^\[MarshalAs\(UnmanagedType.Struct\)(, In)?\]/,"",par)
                gsub(/^By/,      "Selenium.By",      par)
                gsub(/^Strategy/,"Selenium.Strategy",par)
                gsub(/^WebEleme/,"Selenium.WebEleme",par)
                gsub(/^string/,  "System.String",    par)
                gsub(/^object/,  "System.Object",    par)
                gsub(/^bool/,    "System.Boolean",   par)
                gsub(/^double/,  "System.Double",    par)
                gsub(/^float/,   "System.Single",    par)
                gsub(/^int/,     "System.Int32",     par)
                gsub(/^short/,   "System.Int16",     par)
                ps = ps ? ps "," par : par
            }
            if( ps ) ps = "(" ps ")"
            mn = "M:Selenium.ComInterfaces." f[1] "." method ps
        } else
        if( match(NL,/\s([A-Z][A-z]+)\s*{\s*[gs]et/,m) ) {
            mn = "P:Selenium.ComInterfaces." f[1] "." m[1]
        }
        if( mn ) {
            print "        <member name=\"" mn "\">"
            print "            <summary>"
            print "            " descr
            print "            </summary>"
            print "        </member>"
        }
    }
}

END {
    print "    </members>"
    print "</doc>"
}
