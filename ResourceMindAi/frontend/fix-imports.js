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
      if (file.endsWith('.ts')) {
        results.push(file);
      }
    }
  });
  return results;
}

const dir = path.join(__dirname, 'src', 'app', 'features');
const files = walk(dir);

files.forEach(file => {
  let content = fs.readFileSync(file, 'utf8');
  if (content.includes('../../../../shared/')) {
    content = content.replace(/\.\.\/\.\.\/\.\.\/\.\.\/shared\//g, '../../../shared/');
    fs.writeFileSync(file, content, 'utf8');
    console.log('Fixed', file);
  }
  if (content.includes('../../../../core/')) {
    content = content.replace(/\.\.\/\.\.\/\.\.\/\.\.\/core\//g, '../../../core/');
    fs.writeFileSync(file, content, 'utf8');
    console.log('Fixed core in', file);
  }
});
