import React,{useState}from 'react';
export function Switch({label,defaultChecked=false,disabled=false}){
const [on,setOn]=useState(defaultChecked);
return React.createElement('label',{style:{display:'inline-flex',alignItems:'center',gap:8,cursor:disabled?'not-allowed':'pointer',opacity:disabled?0.4:1,fontFamily:'var(--font-sans-text)',fontSize:'var(--fs-body)',color:'var(--text-primary)'}},
React.createElement('span',{onClick:()=>!disabled&&setOn(v=>!v),style:{width:36,height:20,borderRadius:'var(--radius-pill)',background:on?'var(--brand-default)':'var(--neutral-grey-90)',position:'relative',transition:'background var(--duration-fast) var(--ease-standard)'}},
React.createElement('span',{style:{position:'absolute',top:2,left:on?18:2,width:16,height:16,borderRadius:'50%',background:'var(--neutral-white)',transition:'left var(--duration-fast) var(--ease-standard)'}})),
label);
}
