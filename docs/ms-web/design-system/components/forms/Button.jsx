import React from 'react';
const sizeMap={small:{h:24,px:12,fs:'var(--fs-body-sm)'},medium:{h:32,px:16,fs:'var(--fs-body)'},large:{h:40,px:20,fs:'var(--fs-body-lg)'}};
export function Button({children,variant='primary',size='medium',disabled=false,icon,onClick}){
const s=sizeMap[size]||sizeMap.medium;
const base={display:'inline-flex',alignItems:'center',justifyContent:'center',gap:8,height:s.h,padding:`0 ${s.px}px`,
fontFamily:'var(--font-sans-text)',fontSize:s.fs,fontWeight:600,borderRadius:'var(--radius-md)',border:'1px solid transparent',
cursor:disabled?'not-allowed':'pointer',opacity:disabled?0.4:1,transition:'background var(--duration-fast) var(--ease-standard)'};
const variants={
primary:{background:'var(--brand-default)',color:'var(--text-on-brand)'},
secondary:{background:'var(--surface-card)',color:'var(--text-primary)',border:'1px solid var(--border-strong)'},
subtle:{background:'transparent',color:'var(--brand-default)'},
danger:{background:'var(--color-danger)',color:'var(--text-on-brand)'}};
return React.createElement('button',{style:{...base,...variants[variant]},disabled,onClick},
icon&&React.createElement('img',{src:icon,alt:'',style:{width:16,height:16,filter:variant==='primary'||variant==='danger'?'invert(1)':'none'}}),
children);
}
