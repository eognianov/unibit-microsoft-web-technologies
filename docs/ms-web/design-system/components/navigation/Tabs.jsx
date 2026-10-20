import React,{useState}from 'react';
export function Tabs({items,defaultActive=0}){
const [active,setActive]=useState(defaultActive);
return React.createElement('div',{style:{display:'flex',flexDirection:'column',fontFamily:'var(--font-sans-text)',width:360}},
React.createElement('div',{style:{display:'flex',gap:24,borderBottom:'1px solid var(--border-default)'}},
items.map((it,i)=>React.createElement('button',{key:i,onClick:()=>setActive(i),style:{background:'none',border:'none',cursor:'pointer',
padding:'8px 0',fontSize:'var(--fs-body)',fontWeight:i===active?600:400,color:i===active?'var(--brand-default)':'var(--text-secondary)',
borderBottom:i===active?'2px solid var(--brand-default)':'2px solid transparent'}},it))),
React.createElement('div',{style:{padding:'12px 0',fontSize:'var(--fs-body)',color:'var(--text-primary)'}},'Content for '+items[active]));
}
