import React from 'react';
export function Card({title,subtitle,children,elevated=false}){
return React.createElement('div',{style:{background:'var(--surface-card)',borderRadius:'var(--radius-lg)',
border:elevated?'none':'1px solid var(--border-default)',boxShadow:elevated?'var(--shadow-8)':'none',
padding:'var(--space-5)',display:'flex',flexDirection:'column',gap:8,fontFamily:'var(--font-sans-text)',maxWidth:320}},
title&&React.createElement('h3',{style:{margin:0,fontFamily:'var(--font-sans)',fontSize:'var(--fs-title-3)',fontWeight:'var(--fw-title-3)',color:'var(--text-primary)'}},title),
subtitle&&React.createElement('p',{style:{margin:0,fontSize:'var(--fs-body-sm)',color:'var(--text-secondary)'}},subtitle),
children);
}
