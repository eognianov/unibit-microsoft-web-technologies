import React,{useState}from 'react';
export function Checkbox({label,defaultChecked=false,disabled=false}){
const [checked,setChecked]=useState(defaultChecked);
return React.createElement('label',{style:{display:'inline-flex',alignItems:'center',gap:8,cursor:disabled?'not-allowed':'pointer',opacity:disabled?0.4:1,fontFamily:'var(--font-sans-text)',fontSize:'var(--fs-body)',color:'var(--text-primary)'}},
React.createElement('span',{onClick:()=>!disabled&&setChecked(c=>!c),style:{width:18,height:18,borderRadius:'var(--radius-sm)',border:`1.5px solid ${checked?'var(--brand-default)':'var(--border-strong)'}`,
background:checked?'var(--brand-default)':'var(--surface-card)',display:'inline-flex',alignItems:'center',justifyContent:'center',transition:'background var(--duration-fast) var(--ease-standard)'}},
checked&&React.createElement('img',{src:'https://cdn.jsdelivr.net/npm/@fluentui/svg-icons/icons/checkmark_16_filled.svg',alt:'',style:{width:12,height:12,filter:'invert(1)'}})),
label);
}
