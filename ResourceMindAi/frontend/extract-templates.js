const fs = require('fs');
const path = require('path');

function walk(dir) {
  let results = [];
  const list = fs.readdirSync(dir);
  list.forEach(function(file) {
    file = path.join(dir, file);
    const stat = fs.statSync(file);
    if (stat && stat.isDirectory()) {
      results = results.concat(walk(file));
    } else {
      if (file.endsWith('.component.ts') && !file.includes('node_modules')) {
        results.push(file);
      }
    }
  });
  return results;
}

const dir = path.join(__dirname, 'src', 'app');
const files = walk(dir);

let processed = 0;

files.forEach(file => {
  let content = fs.readFileSync(file, 'utf8');
  
  // Find template: `...` block
  // We use a regex that handles backticks across multiple lines
  const templateRegex = /template:\s*`([\s\S]*?)`\n?(\s*})?/g;
  const match = templateRegex.exec(content);
  
  if (match) {
    const templateContent = match[1];
    
    // Determine base name (e.g. login.component.ts -> login.component)
    const basename = path.basename(file, '.ts');
    const dirname = path.dirname(file);
    
    const htmlPath = path.join(dirname, basename + '.html');
    const cssPath = path.join(dirname, basename + '.css');
    
    // Write HTML and CSS
    fs.writeFileSync(htmlPath, templateContent.trim() + '\n', 'utf8');
    fs.writeFileSync(cssPath, '', 'utf8');
    
    // Replace template with templateUrl and styleUrl
    const replacement = `templateUrl: './${basename}.html',\n  styleUrl: './${basename}.css'\n$2`;
    
    const newContent = content.replace(templateRegex, replacement);
    fs.writeFileSync(file, newContent, 'utf8');
    
    console.log('Processed:', file);
    processed++;
  }
});

console.log(`Done. Processed ${processed} files.`);
