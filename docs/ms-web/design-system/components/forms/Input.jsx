import React,{useState}from 'react';
export function Input({label,placeholder,defaultValue='',error,disabled=false,type='text'}){
const [focused,setFocused]=useState(false);
const id='inp-'+Math.random().toString(36).slice(2,8);
return React.createElement('div',{style:{display:'flex',flexDirection:'column',gap:4,fontFamily:'var(--font-sans-text)',width:240}},
label&&React.createElement('label',{htmlFor:id,style:{fontSize:'var(--fs-body-sm)',color:'var(--text-secondary)'}},label),
React.createElement('input',{id,type,placeholder,defaultValue,disabled,onFocus:()=>setFocused(true),onBlur:()=>setFocused(false),
style:{height:32,padding:'0 10px',fontSize:'var(--fs-body)',borderRadius:'var(--radius-md)',
border:`1px solid ${error?'var(--color-danger)':focused?'var(--brand-default)':'var(--border-strong)'}`,
outline:focused?`1px solid var(--brand-default)`:'none',background:disabled?'var(--surface-subtle)':'var(--surface-card)',color:'var(--text-primary)'}}),
error&&React.createElement('span',{style:{fontSize:'var(--fs-caption)',color:'var(--color-danger)'}},error));
}
