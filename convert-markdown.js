import { readFileSync, writeFileSync } from 'fs';
import { fileURLToPath } from 'url';
import { dirname, join } from 'path';

const __filename = fileURLToPath(import.meta.url);
const __dirname = dirname(__filename);

// Basit Markdown to HTML converter
function markdownToHTML(markdown) {
  let html = markdown;
  
  // Headers
  html = html.replace(/^### (.*$)/gim, '<h3>$1</h3>');
  html = html.replace(/^## (.*$)/gim, '<h2>$1</h2>');
  html = html.replace(/^# (.*$)/gim, '<h1>$1</h1>');
  
  // Bold
  html = html.replace(/\*\*(.*?)\*\*/g, '<strong>$1</strong>');
  
  // Code blocks
  html = html.replace(/```(\w+)?\n([\s\S]*?)```/g, (match, lang, code) => {
    const language = lang || 'text';
    return `<pre><code class="language-${language}">${escapeHtml(code.trim())}</code></pre>`;
  });
  
  // Inline code
  html = html.replace(/`([^`]+)`/g, '<code>$1</code>');
  
  // Links
  html = html.replace(/\[([^\]]+)\]\(([^)]+)\)/g, '<a href="$2">$1</a>');
  
  // Lists
  html = html.replace(/^\- (.*$)/gim, '<li>$1</li>');
  html = html.replace(/^(\d+)\. (.*$)/gim, '<li>$2</li>');
  html = html.replace(/(<li>.*<\/li>)/s, '<ul>$1</ul>');
  
  // Paragraphs
  html = html.split('\n\n').map(para => {
    if (!para.trim()) return '';
    if (para.startsWith('<')) return para;
    return `<p>${para}</p>`;
  }).join('\n');
  
  // Horizontal rules
  html = html.replace(/^---$/gim, '<hr>');
  
  // Line breaks
  html = html.replace(/\n/g, '<br>');
  
  return html;
}

function escapeHtml(text) {
  const map = {
    '&': '&amp;',
    '<': '&lt;',
    '>': '&gt;',
    '"': '&quot;',
    "'": '&#039;'
  };
  return text.replace(/[&<>"']/g, m => map[m]);
}

// Read markdown file
const markdownPath = join(__dirname, 'ZOD_REHBERI.md');
const markdown = readFileSync(markdownPath, 'utf-8');

// Convert to HTML
const htmlContent = markdownToHTML(markdown);

// Create full HTML document
const fullHTML = `<!DOCTYPE html>
<html lang="tr">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Zod Kullanım Rehberi</title>
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/highlight.js/11.9.0/styles/github-dark.min.css">
    <script src="https://cdnjs.cloudflare.com/ajax/libs/highlight.js/11.9.0/highlight.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/highlight.js/11.9.0/languages/typescript.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/highlight.js/11.9.0/languages/javascript.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/highlight.js/11.9.0/languages/bash.min.js"></script>
    <style>
        * {
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }
        
        body {
            font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif;
            line-height: 1.6;
            color: #333;
            background: #f5f5f5;
            padding: 20px;
        }
        
        .container {
            max-width: 1200px;
            margin: 0 auto;
            background: white;
            padding: 40px;
            border-radius: 8px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.1);
        }
        
        h1 {
            color: #2c3e50;
            border-bottom: 3px solid #3498db;
            padding-bottom: 10px;
            margin-bottom: 30px;
            font-size: 2.5em;
        }
        
        h2 {
            color: #34495e;
            margin-top: 40px;
            margin-bottom: 20px;
            padding-top: 20px;
            border-top: 2px solid #ecf0f1;
            font-size: 2em;
        }
        
        h3 {
            color: #7f8c8d;
            margin-top: 30px;
            margin-bottom: 15px;
            font-size: 1.5em;
        }
        
        p {
            margin-bottom: 15px;
            text-align: justify;
        }
        
        code {
            background: #f4f4f4;
            padding: 2px 6px;
            border-radius: 3px;
            font-family: 'Courier New', monospace;
            font-size: 0.9em;
            color: #e83e8c;
        }
        
        pre {
            background: #282c34;
            padding: 20px;
            border-radius: 6px;
            overflow-x: auto;
            margin: 20px 0;
            box-shadow: 0 2px 8px rgba(0,0,0,0.1);
        }
        
        pre code {
            background: none;
            padding: 0;
            color: #abb2bf;
            font-size: 0.9em;
        }
        
        ul, ol {
            margin-left: 30px;
            margin-bottom: 20px;
        }
        
        li {
            margin-bottom: 8px;
        }
        
        a {
            color: #3498db;
            text-decoration: none;
        }
        
        a:hover {
            text-decoration: underline;
        }
        
        hr {
            border: none;
            border-top: 2px solid #ecf0f1;
            margin: 30px 0;
        }
        
        strong {
            color: #2c3e50;
            font-weight: 600;
        }
        
        .toc {
            background: #f8f9fa;
            padding: 20px;
            border-radius: 6px;
            margin-bottom: 30px;
            border-left: 4px solid #3498db;
        }
        
        .toc h2 {
            margin-top: 0;
            border: none;
            padding: 0;
        }
        
        .toc ul {
            list-style: none;
            margin-left: 0;
        }
        
        .toc a {
            color: #34495e;
            display: block;
            padding: 5px 0;
        }
        
        .toc a:hover {
            color: #3498db;
        }
    </style>
</head>
<body>
    <div class="container">
        ${htmlContent}
    </div>
    <script>
        // Highlight code blocks
        document.addEventListener('DOMContentLoaded', function() {
            document.querySelectorAll('pre code').forEach((block) => {
                hljs.highlightElement(block);
            });
        });
    </script>
</body>
</html>`;

// Write HTML file
const htmlPath = join(__dirname, 'ZOD_REHBERI.html');
writeFileSync(htmlPath, fullHTML, 'utf-8');

console.log('✅ HTML dosyası oluşturuldu: ZOD_REHBERI.html');

