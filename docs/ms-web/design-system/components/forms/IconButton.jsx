import React from 'react';
export function IconButton({icon,label,variant='secondary',size=32,onClick}){
const variants={primary:{background:'var(--brand-default)',filter:'invert(1)'},secondary:{background:'var(--surface-card)',border:'1px solid var(--border-strong)',filter:'none'},subtle:{background:'transparent',border:'none',filter:'none'}};
const v=variants[variant]||variants.secondary;
return React.createElement('button',{'aria-label':label,title:label,style:{width:size,height:size,display:'inline-flex',alignItems:'center',justifyContent:'center',
borderRadius:'var(--radius-md)',border:v.border||'1px solid transparent',background:v.background,cursor:'pointer',transition:'background var(--duration-fast) var(--ease-standard)'},onClick},
React.createElement('img',{src:icon,alt:'',style:{width:size*0.55,height:size*0.55,filter:v.filter}}));
}
